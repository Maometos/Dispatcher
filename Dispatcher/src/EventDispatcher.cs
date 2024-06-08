namespace Dispatcher;

public class EventDispatcher
{
    private Dictionary<Type, object> serviceProvider = new();
    private Dictionary<int, Type> handlerProvider = new();
    private Dictionary<int, Dictionary<int, Type>> listenerProvider = new();

    public EventDispatcher() { }

    public EventDispatcher(object[] services)
    {
        foreach (var service in services)
        {
            AddService(service);
        }
    }

    public void AddService(object service)
    {
        serviceProvider[service.GetType()] = service;
    }

    public void AddHandler<THandler>()
    {
        AddHandler(typeof(THandler));
    }

    public void AddHandler(Type handlerType)
    {
        var eventType = GetEventType(handlerType, typeof(RequestHandler<,>));
        if (eventType == null)
        {
            throw new Exception("The given type '" + handlerType.Name + "' must extends the abstract request handler: " + typeof(RequestHandler<,>).FullName);
        }

        handlerProvider[eventType.GetHashCode()] = handlerType;
    }

    public void AddListener<TListener>()
    {
        AddListener(typeof(TListener));
    }

    public void AddListener(Type ListenerType)
    {
        var notificationType = GetEventType(ListenerType, typeof(NotificationListener<>));
        if (notificationType == null)
        {
            throw new Exception("The given type '" + ListenerType.Name + "' must extends the abstract notification listener: " + typeof(NotificationListener<>).FullName);
        }

        if (!listenerProvider.ContainsKey(notificationType.GetHashCode()))
        {
            listenerProvider[notificationType.GetHashCode()] = new Dictionary<int, Type>();
        }

        listenerProvider[notificationType.GetHashCode()][ListenerType.GetHashCode()] = ListenerType;
    }

    private Type? GetEventType(Type handlerType, Type targetType)
    {
        var baseType = handlerType.BaseType;

        if (baseType == null)
        {
            return null;
        }

        if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == targetType)
        {
            return baseType.GetGenericArguments()[0];
        }

        return GetEventType(baseType, targetType);
    }

    public async Task<object?> DispatchAsync(IEvent eventArgs, CancellationToken token = default)
    {
        if (handlerProvider.ContainsKey(eventArgs.GetType().GetHashCode()))
        {
            var handlerType = handlerProvider[eventArgs.GetType().GetHashCode()];
            var handler = CreateInstance<IEventHandler>(handlerType);

            return await handler.InvokeAsync(eventArgs, token);
        }

        if (listenerProvider.ContainsKey(eventArgs.GetType().GetHashCode()))
        {
            var listenerTypes = listenerProvider[eventArgs.GetType().GetHashCode()];

            var tasks = new List<Task>();

            foreach (var item in listenerTypes)
            {
                var listener = CreateInstance<IEventListener>(item.Value);
                var task = listener.InvokeAsync(eventArgs, token);
                tasks.Add(task);
            }

            foreach (var task in tasks)
            {
                await task;
            }

            return null;
        }

        throw new Exception("No registered handler or listener matches the given event: " + eventArgs.GetType().Name);
    }

    private TInstance CreateInstance<TInstance>(Type handlerType)
    {
        var constructor = handlerType.GetConstructors()[0];

        var args = new List<object>();
        var parameters = constructor.GetParameters();
        foreach (var parameter in parameters)
        {
            var parameterType = parameter.ParameterType;
            if (!serviceProvider.ContainsKey(parameterType))
            {
                throw new Exception("Cannot find a corresponding dependency of type '" + parameterType.Name + "' for the request handler: " + handlerType.FullName);
            }

            args.Add(serviceProvider[parameterType]);
        }

        if (args.Count > 0)
        {
            return (TInstance)Activator.CreateInstance(handlerType, args.ToArray())!;
        }

        return (TInstance)Activator.CreateInstance(handlerType)!;
    }
}
