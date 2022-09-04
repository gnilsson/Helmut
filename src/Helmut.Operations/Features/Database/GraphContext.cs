using NRedisGraph;
using StackExchange.Redis;
using System.Reflection;

namespace Helmut.Operations.Features.Database;

public class AsyncGraphProxy<TDecorated> : DispatchProxyAsync where TDecorated : class, IPinger
{
    private TDecorated _decorated = null!;
    private ILogger<AsyncGraphProxy<TDecorated>> _logger = null!;

    public override object Invoke(MethodInfo method, object[] args)
    {
        throw new NotImplementedException();
    }

    public override Task InvokeAsync(MethodInfo method, object[] args)
    {
        throw new NotImplementedException();
    }

    public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
    {
        _logger.LogInformation("Hej2");
        var result = await (Task<T>)method.Invoke(_decorated, args)!;
        _logger.LogInformation("Completed2");
        return result;
    }

    public static TDecorated Create(TDecorated decorated, ILogger<AsyncGraphProxy<TDecorated>> logger)
    {
        object proxy = Create<TDecorated, AsyncGraphProxy<TDecorated>>();
        ((AsyncGraphProxy<TDecorated>)proxy).SetParameters(decorated, logger);

        return (TDecorated)proxy;
    }

    private void SetParameters(TDecorated decorated, ILogger<AsyncGraphProxy<TDecorated>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
    }
}

public class GraphProxy<TDecorated> : DispatchProxy where TDecorated : class, IPinger
{
    private TDecorated _decorated = null!;
    private ILogger<GraphProxy<TDecorated>> _logger = null!;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        _logger.LogInformation("Hej");

        var result = targetMethod!.Invoke(_decorated, args);

        _logger.LogInformation("Completed");

        return result;
    }

    public static TDecorated Create(TDecorated decorated, ILogger<GraphProxy<TDecorated>> logger)
    {
        object proxy = Create<TDecorated, GraphProxy<TDecorated>>();
        ((GraphProxy<TDecorated>)proxy).SetParameters(decorated, logger);

        return (TDecorated)proxy;
    }

    private void SetParameters(TDecorated decorated, ILogger<GraphProxy<TDecorated>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
    }
}

public interface IPinger
{
    Task<int> PingAsync();
}

public class GraphContext : IPinger
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<GraphContext> _logger;

    public GraphContext(IConnectionMultiplexer redis, ILogger<GraphContext> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<int> PingAsync()
    {
        _logger.LogInformation("Ping");
        return 1;
        //var db = _redis.GetDatabase();
        //var pong = await db.PingAsync();
        //_logger.LogInformation("pong: {pong}", pong.ToString());
        //var graph = new RedisGraph(db);
        //var f = await graph.QueryAsync("test", "CREATE (:test {name:'hej'})");
        //var b = await graph.QueryAsync("test", "MATCH (n {name:'hej'}) RETURN n");
    }
}
