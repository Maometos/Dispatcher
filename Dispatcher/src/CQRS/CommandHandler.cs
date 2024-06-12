namespace Dispatcher.CQRS;

public abstract class CommandHandler<TCommand> : RequestHandler<TCommand, int> where TCommand : Command
{
    protected abstract Task<int> CreateAsync(TCommand command, CancellationToken token);
    protected abstract Task<int> UpdateAsync(TCommand command, CancellationToken token);
    protected abstract Task<int> DeleteAsync(TCommand command, CancellationToken token);

    public override Task<int> InvokeAsync(TCommand command, CancellationToken token)
    {
        switch (command.Action)
        {
            case CommandAction.Create: return CreateAsync(command, token);
            case CommandAction.Update: return UpdateAsync(command, token);
            case CommandAction.Delete: return DeleteAsync(command, token);
        }

        return Task.FromResult(0);
    }
}
