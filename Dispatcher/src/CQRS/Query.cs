namespace Dispatcher.CQRS;

public abstract class Query : Request
{
    public QueryAction Action { get; set; } = QueryAction.None;
    public string Sort { get; set; } = "";
    public bool Reverse { get; set; } = false;
    public int Page { get; set; } = 0;
    public int Limit { get; set; } = 0;
}
