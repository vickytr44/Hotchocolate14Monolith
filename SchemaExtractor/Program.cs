using HotChocolate.Execution;
using HotChocolateV14.Queries;
using HotChocolateV14.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SchemaExtractor;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ResolveRepositoryDependencies();

var schema = (await (builder.Services
    .AddGraphQLServer()
    .AddQueryType(x => x.Name("Query"))
    .AddTypeExtension<CustomerQuery>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .ModifyCostOptions(o => o.EnforceCostLimits = false)).BuildSchemaAsync()).Print();


Console.WriteLine(schema);

var cleanedSchema = GraphQLSchemaCleaner.CleanGraphQLSchema(schema);

File.WriteAllText(@"C:\HotChocolateGraphQL14\SchemaExtractor\Files\cleanedSchema.txt", cleanedSchema);

var fullSchema = (await (builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddPagingArguments()
    .ModifyCostOptions(o => o.EnforceCostLimits = false)).BuildSchemaAsync()).Print();

var schemaJson = SchemaConverter.ConvertGraphQLSchemaToJson(fullSchema);

foreach (var item in schemaJson)
{
    var content = JsonSerializer.Serialize(item.Value, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText($"C:\\HotChocolateGraphQL14\\SchemaExtractor\\Files\\{item.Key}.json", content);
}