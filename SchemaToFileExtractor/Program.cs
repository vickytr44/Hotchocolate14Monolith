using HotChocolate.Execution;
using HotChocolateV14.Queries;
using HotChocolateV14.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using SchemaToFileExtractor;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var filePath = @"C:\HotChocolateGraphQL14\HotChocolateV14\SchemaToFileExtractor\Files\";

builder.Services.ResolveRepositoryDependencies();

var typeName = "CustomerQuery";

var assembly = Assembly.Load("HotChocolateV14");

Type type = assembly.GetTypes().ToList().First(x => x.Name == typeName);

Console.WriteLine();

var schema = (await (builder.Services
    .AddGraphQLServer()
    .AddQueryType(x => x.Name("Query"))
    .AddTypeExtension(type)
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .ModifyCostOptions(o => o.EnforceCostLimits = false)).BuildSchemaAsync()).Print();


Console.WriteLine(schema);

var cleanedSchema = GraphQLSchemaCleaner.CleanGraphQLSchema(schema);

File.WriteAllText(filePath + "cleanedSchema.txt", cleanedSchema);

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
    File.WriteAllText(filePath + $"{item.Key}.json", content);
}