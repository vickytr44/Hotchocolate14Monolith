namespace HotChocolateV14.JsonToLinqConverter;
public static class EfEntityResolver
{
    public static Type ResolveEntityType(ApplicationContext dbContext, string entityName)
    {
        var entityTypes = dbContext.Model
           .GetEntityTypes();

        var entityType = entityTypes
            .FirstOrDefault(e =>
                string.Equals(
                    e.ClrType.Name,
                    entityName,
                    StringComparison.OrdinalIgnoreCase));

        if (entityType == null)
            throw new InvalidOperationException(
                $"Entity '{entityName}' not found in DbContext model");

        return entityType.ClrType;
    }
}
