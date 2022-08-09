namespace Helmut.General;

public interface IBackgroundTaskQueue<T>
{
    ValueTask QueueTaskAsync(T workItem);
    ValueTask<T> DequeueTaskAsync(CancellationToken cancellationToken);
}
