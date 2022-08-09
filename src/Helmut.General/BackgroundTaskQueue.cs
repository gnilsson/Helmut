using System.Threading.Channels;

namespace Helmut.General;

public abstract class BackgroundTaskQueue<T> : IBackgroundTaskQueue<T>
{
    private readonly Channel<T> _queue;

    public BackgroundTaskQueue(int capacity = 100, BoundedChannelFullMode mode = BoundedChannelFullMode.Wait)
    {
        // Capacity should be set based on the expected application load and
        // number of concurrent threads accessing the queue.
        // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
        // which completes only when space became available. This leads to backpressure,
        // in case too many publishers/calls start accumulating.
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = mode
        };

        _queue = Channel.CreateBounded<T>(options);
    }

    public async ValueTask QueueTaskAsync(T workItem)
    {
        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<T> DequeueTaskAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}
