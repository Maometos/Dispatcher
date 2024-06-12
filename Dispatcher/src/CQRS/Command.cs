namespace Dispatcher.CQRS;

public abstract class Command : Request
{
    public CommandAction Action { get; set; } = CommandAction.None;
}
