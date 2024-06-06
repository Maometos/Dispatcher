namespace Dispatcher;

public abstract class NotificationListener<TNotification> : IEventListener where TNotification : Notification
{
    public async Task InvokeAsync(IEvent notification, CancellationToken token)
    {
        await InvokeAsync((TNotification)notification, token);
    }

    public abstract Task InvokeAsync(TNotification notification, CancellationToken token);
}
