using HotChocolateV14;
using HotChocolateV14.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<ApplicationContext>(
        options => options.UseSqlServer("Server=.;Database=DemoGraphQl;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddPagingArguments();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL();
app.Run();
