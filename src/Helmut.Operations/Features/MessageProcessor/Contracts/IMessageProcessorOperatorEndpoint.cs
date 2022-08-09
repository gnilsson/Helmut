using Helmut.Operations.Features.MessageProcessor;

namespace Helmut.Operations.Features.MessageProcessor.Contracts;

public interface IMessageProcessorOperatorEndpoint
{
    ValueTask ExecuteAsync(MessageProcessorOperationRequest request, CancellationToken cancellationToken);
}
