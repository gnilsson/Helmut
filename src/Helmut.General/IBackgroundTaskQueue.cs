namespace Helmut.General;

public interface IBackgroundTaskQueue<T>
{
    ValueTask QueueBackgroundWorkItemAsync(T workItem);
    ValueTask<T> DequeueAsync(CancellationToken cancellationToken);
}
