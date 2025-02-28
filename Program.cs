using HotChocolateV14;
using HotChocolateV14.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services
    .AddDbContextPool<ApplicationContext>(
        options => options.UseSqlServer("Server=.;Database=DemoGraphQl;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddPagingArguments()
    .ModifyCostOptions(o => o.EnforceCostLimits = false);

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.MapGraphQL();
app.Run();
