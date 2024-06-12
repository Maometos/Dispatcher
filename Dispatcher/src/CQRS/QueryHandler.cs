namespace Dispatcher.CQRS;

public abstract class QueryHandler<TQuery, TEntity> : RequestHandler<TQuery, List<TEntity>> where TQuery : Query
{
    protected abstract Task<TEntity?> FindAsync(TQuery query, CancellationToken token);
    protected abstract Task<List<TEntity>> ListAsync(TQuery query, CancellationToken token);

    public override async Task<List<TEntity>> InvokeAsync(TQuery query, CancellationToken token)
    {
        var list = new List<TEntity>();
        switch (query.Action)
        {
            case QueryAction.Find:
                var entity = await FindAsync(query, token);
                if (entity != null) list.Add(entity);
                break;
            case QueryAction.List: return await ListAsync(query, token);
        }

        return list;
    }
}
