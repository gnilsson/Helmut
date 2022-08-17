using Azure.Messaging.ServiceBus;
using Helmut.General.Models;
using Helmut.Operations.Features.LocationTranscoder;
using Helmut.Operations.Features.MessageProcessor.Contracts;
using System.Text;
using System.Text.Json;

namespace Helmut.Operations.Features.MessageProcessor;

public sealed class MessageProcessorService : BackgroundService
{
    private readonly ILogger<MessageProcessorService> _logger;
    private readonly ServiceBusClient _client;
    private readonly IConfiguration _configuration;
    private readonly IMessageProcessorTaskQueue _taskQueue;
    private readonly ILocationTranscoderService _locationTranscoderService;

    private int _executionCount;

    public MessageProcessorService(
        ILogger<MessageProcessorService> logger,
        ServiceBusClient client,
        IConfiguration configuration,
        IMessageProcessorTaskQueue taskQueue,
        ILocationTranscoderService locationTranscoderService)
    {
        _logger = logger;
        _client = client;
        _configuration = configuration;
        _taskQueue = taskQueue;
        _locationTranscoderService = locationTranscoderService;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aborting service unit {Service}.", nameof(MessageProcessorService));

        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processorOptions = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 2,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            PrefetchCount = 10,
        };

        await using var processor = _client.CreateProcessor(_configuration["AzureServiceBus:QueueName"], processorOptions);

        processor.ProcessMessageAsync += (args) =>
        {
            return ProcessMessageAsync(args);
        };

        processor.ProcessErrorAsync += (args) =>
        {
            _logger.LogInformation("exception was thrown, message: {Message}", args.Exception.Message);

            return Task.CompletedTask;
        };

        await RunProcessorAsync(processor, stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var vessel = await JsonSerializer.DeserializeAsync<Vessel>(args.Message.Body.ToStream(), cancellationToken: args.CancellationToken);

            if (vessel is null)
            {
                await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken);
                return;
            }

            var locationName = await _locationTranscoderService.TranscodeCoordinatesAsync(vessel.Coordinates);

            _logger.LogInformation("Received message with vessel.\nName: {Name}\nGroup: {Group}\nLocationName: {LocationName}", vessel.Affinity.Name, vessel.Affinity?.Group, locationName);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when receiving message.\n{Message}", Encoding.UTF8.GetString(args.Message.Body));

            await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken);
            return;
        }

        Interlocked.Increment(ref _executionCount);

        try
        {
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when completing message.\n{Message}", Encoding.UTF8.GetString(args.Message.Body));
            return;
        }
    }

    private async Task RunProcessorAsync(ServiceBusProcessor processor, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await _taskQueue.DequeueTaskAsync(stoppingToken);

            try
            {
                await task(processor, StopAsync, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred executing {Task}.", nameof(task));
            }
        }
    }
}
