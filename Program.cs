using HotChocolateV14;
using HotChocolateV14.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

builder.Services.ResolveRepositoryDependencies();

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddPagingArguments()
    .ModifyCostOptions(o => o.EnforceCostLimits = false);

builder.Services.AddControllers();

// Add Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotChocolate API", Version = "v1" });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Add Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotChocolate API V1");
});

app.UseHttpsRedirection();

app.MapGraphQL();
app.MapControllers();

app.Run();