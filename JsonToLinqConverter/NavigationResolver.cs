namespace HotChocolateV14.JsonToLinqConverter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public static class NavigationResolver
{
    public static INavigation ResolveNavigation(
        DbContext dbContext,
        Type sourceEntity,
        string targetEntityName)
    {
        var entityType = dbContext.Model.FindEntityType(sourceEntity)
            ?? throw new InvalidOperationException($"Entity '{sourceEntity.Name}' not found");

        var navigation = entityType.GetNavigations()
            .FirstOrDefault(n =>
                string.Equals(
                    n.TargetEntityType.ClrType.Name,
                    targetEntityName,
                    StringComparison.OrdinalIgnoreCase));

        if (navigation == null)
            throw new InvalidOperationException(
                $"No navigation from '{sourceEntity.Name}' to '{targetEntityName}'");

        return navigation;
    }
}


