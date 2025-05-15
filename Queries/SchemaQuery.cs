using HotChocolateV14.Constants;
using HotChocolateV14.Utils;

namespace HotChocolateV14.Queries;

[QueryType]
public class SchemaQuery
{
    public string GetCleanedSchema(Entity schemaName)
    {
        var schemaAsString = File.ReadAllText(FilePath.SchemaFilePath + schemaName.GetDescription() + ".txt");

        return schemaAsString;
    }
}
