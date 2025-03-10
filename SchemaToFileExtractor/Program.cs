using HotChocolate.Execution;
using HotChocolateV14.Constants;
using HotChocolateV14.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SchemaToFileExtractor;
using System.Reflection;
using System.Text.Json;

foreach(var entity in Enum.GetValues(typeof( Entity)).Cast<Entity>())
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.ResolveRepositoryDependencies();

    var typeName = entity + "Query";

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


    var cleanedSchema = GraphQLSchemaCleaner.CleanGraphQLSchema(schema);

    File.WriteAllText(FilePath.SchemaFilePath + entity.ToString().ToLower() + "s" + ".txt", cleanedSchema);
}

var fullSchemaBuilder = WebApplication.CreateBuilder(args);

fullSchemaBuilder.Services.ResolveRepositoryDependencies();

var fullSchema = (await (fullSchemaBuilder.Services
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
    File.WriteAllText(FilePath.JsonFilePath + $"{item.Key}.json", content);
}