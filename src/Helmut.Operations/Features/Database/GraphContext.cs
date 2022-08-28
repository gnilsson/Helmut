using NRedisGraph;
using StackExchange.Redis;

namespace Helmut.Operations.Features.Database;

public class GraphContext
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<GraphContext> _logger;

    public GraphContext(IConnectionMultiplexer redis, ILogger<GraphContext> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task PingAsync()
    {
        var db = _redis.GetDatabase();
        var pong = await db.PingAsync();
        _logger.LogInformation("pong: {pong}", pong.ToString());
        var graph = new RedisGraph(db);
        var f = await graph.QueryAsync("test", "CREATE (:test {name:'hej'})");
        var b = await graph.QueryAsync("test", "MATCH (n {name:'hej'}) RETURN n");
    }
}
