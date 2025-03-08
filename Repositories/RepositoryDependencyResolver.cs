namespace HotChocolateV14.Repositories;

public static class RepositoryDependencyResolver
{
    public static IServiceCollection ResolveRepositoryDependencies(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IBillRepository, BillRepository>();

        return services;
    }
}
