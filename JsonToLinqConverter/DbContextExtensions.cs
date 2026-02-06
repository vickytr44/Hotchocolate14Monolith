using Microsoft.EntityFrameworkCore;

namespace HotChocolateV14.JsonToLinqConverter;

public static class DbContextExtensions
{
    public static IQueryable GetQueryable(this ApplicationContext context, Type entityType)
    {
        var method = typeof(ApplicationContext)
            .GetMethod(nameof(ApplicationContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(entityType);

        return (IQueryable)method.Invoke(context, null)!;
    }
}
