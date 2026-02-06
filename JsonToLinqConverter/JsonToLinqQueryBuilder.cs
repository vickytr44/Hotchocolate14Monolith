using Microsoft.EntityFrameworkCore;

namespace HotChocolateV14.JsonToLinqConverter;

public sealed class JsonToLinqQueryBuilder : IJsonToLinqQueryBuilder
{
    private readonly ApplicationContext _dbContext;

    public JsonToLinqQueryBuilder(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable Build(QueryRequest request)
    {
        var entityType = EfEntityResolver.ResolveEntityType(
            _dbContext,
            request.MainTable);

        var query = _dbContext.GetQueryable(entityType);

        query = ProjectionBuilder.ApplySelect(
                    query,
                    entityType,
                    request.SelectedFields ?? Array.Empty<string>(), 
                    request.RelatedTables,
                    _dbContext);

        return query;
    }
}
