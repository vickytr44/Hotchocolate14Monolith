using HotChocolateV14.Constants;

namespace HotChocolateV14.Queries;

[QueryType]
public class SchemaQuery
{
    public string GetCleanedSchema(string schemaName)
    {
        var schemaAsString = File.ReadAllText(FilePath.SchemaFilePath + schemaName + ".txt");

        return schemaAsString;
    }
}
