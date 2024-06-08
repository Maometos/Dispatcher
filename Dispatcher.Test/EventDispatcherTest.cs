namespace Dispatcher.Test;

public class DispatcherTest
{
    private EventDispatcher dispatcher;

    public DispatcherTest()
    {
        dispatcher = new EventDispatcher();
        dispatcher.AddService(new Response());
        dispatcher.AddHandler<PingHandler>();
        dispatcher.AddHandler<TickHandler>();
        dispatcher.AddListener<AlphaListener>();
        dispatcher.AddListener<BravoListener>();
    }

    [Fact]
    public async void TestDispatchingRequest()
    {
        var ping = new PingRequest();
        var pong = (Response?)await dispatcher.DispatchAsync(ping);
        Assert.Equal("Pong", pong?.Value);

        var tick = new TickRequest();
        var tock = await dispatcher.DispatchAsync(tick);
        Assert.Equal("Tock", tock);
    }

    [Fact]
    public async void TestDispatchingNotification()
    {
        var notification = new Subject();
        await dispatcher.DispatchAsync(notification);
        Assert.Equal(2, notification.Value);
    }
}

public class Response
{
    public string Value { get; set; } = "";
}

public class PingRequest : Request
{
    public string Name { get; set; } = "Ping";
}

public class PingHandler : RequestHandler<PingRequest, Response>
{
    private Response response;

    public PingHandler(Response response)
    {
        this.response = response;
    }

    public override Task<Response> InvokeAsync(PingRequest request, CancellationToken token = default)
    {
        response.Value = "Pong";
        return Task.FromResult(response);
    }
}

public class TickRequest : Request
{
    public string Name { get; set; } = "Tick";
}

public class TickHandler : RequestHandler<TickRequest, string>
{
    public override Task<string> InvokeAsync(TickRequest request, CancellationToken token = default)
    {
        return Task.FromResult("Tock");
    }
}

public class Subject : Notification
{
    public int Value { get; set; } = 0;
}

public class AlphaListener : NotificationListener<Subject>
{
    public override Task InvokeAsync(Subject notification, CancellationToken token = default)
    {
        notification.Value++;
        return Task.CompletedTask;
    }
}

public class BravoListener : NotificationListener<Subject>
{
    public override Task InvokeAsync(Subject notification, CancellationToken token = default)
    {
        notification.Value++;
        return Task.CompletedTask;
    }
}
