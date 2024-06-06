namespace Dispatcher;

public interface IEventListener
{
    public Task InvokeAsync(IEvent eventArgs, CancellationToken token);
}
