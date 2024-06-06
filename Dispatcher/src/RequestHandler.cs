namespace Dispatcher;

public abstract class RequestHandler<TRequest, TResponse> : IEventHandler where TRequest : Request
{
    public async Task<object?> InvokeAsync(IEvent request, CancellationToken token)
    {
        return await InvokeAsync((TRequest)request, token);
    }

    public abstract Task<TResponse> InvokeAsync(TRequest request, CancellationToken token);
}
