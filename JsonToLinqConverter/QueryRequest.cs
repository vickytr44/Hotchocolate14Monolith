namespace HotChocolateV14.JsonToLinqConverter;

public sealed class QueryRequest
{
    public string MainTable { get; set; }
    public IReadOnlyList<string>? SelectedFields { get; set; }
    public IReadOnlyList<RelatedTableRequest>? RelatedTables { get; set; }
}

public sealed class RelatedTableRequest
{
    public string Entity { get; set; } 
    public IReadOnlyList<string> SelectedFields { get; set; } = [];
}