using System.Text.Json;
using HotChocolate.Language;
using HotChocolate.Types.Descriptors.Definitions;

namespace SchemaExtractor;

public static class SchemaConverter
{
    public static Dictionary<string, object> ConvertGraphQLSchemaToJson(string schema)
    {
        var document = Utf8GraphQLParser.Parse(schema);
        var schemaJson = new Dictionary<string, object>
        {
            ["schema"] = new Dictionary<string, object>(),
            ["types"] = new Dictionary<string, object>(),
            ["inputs"] = new Dictionary<string, object>(),
            ["enums"] = new Dictionary<string, object>()
        };

        foreach (var definition in document.Definitions)
        {
            switch (definition)
            {
                case ObjectTypeDefinitionNode typeDef:
                    var fields = new Dictionary<string, object>();
                    foreach (var field in typeDef.Fields)
                    {
                        fields[field.Name.Value] = GetFieldType(field.Type);
                    }
                    ((Dictionary<string, object>)schemaJson["types"])[typeDef.Name.Value] = fields;
                    break;

                case InputObjectTypeDefinitionNode inputDef:
                    var inputFields = new Dictionary<string, object>();
                    foreach (var field in inputDef.Fields)
                    {
                        inputFields[field.Name.Value] = GetFieldType(field.Type);
                    }
                    ((Dictionary<string, object>)schemaJson["inputs"])[inputDef.Name.Value] = inputFields;
                    break;

                case EnumTypeDefinitionNode enumDef:
                    var enumValues = new List<string>();
                    foreach (var value in enumDef.Values)
                    {
                        enumValues.Add(value.Name.Value);
                    }
                    ((Dictionary<string, object>)schemaJson["enums"])[enumDef.Name.Value] = enumValues;
                    break;

                case SchemaDefinitionNode schemaDef:
                    var queryType = schemaDef.OperationTypes.First(x => x.Operation == OperationType.Query);
                    if (queryType != null)
                    {
                        ((Dictionary<string, object>)schemaJson["schema"])["query"] = queryType.Type.Name.Value;
                    }
                    break;
            }
        }

        return schemaJson;
    }

    private static object GetFieldType(ITypeNode type)
    {
        return type switch
        {
            NonNullTypeNode nonNull => GetFieldType(nonNull.Type) + "!",
            ListTypeNode listType => "[" + GetFieldType(listType.Type) + "]",
            NamedTypeNode namedType => namedType.Name.Value,
            _ => "Unknown"
        };
    }
}


