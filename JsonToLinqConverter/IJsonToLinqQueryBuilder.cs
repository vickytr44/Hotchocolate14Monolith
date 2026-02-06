namespace HotChocolateV14.JsonToLinqConverter;

public interface IJsonToLinqQueryBuilder
{
    IQueryable Build(QueryRequest request);
}