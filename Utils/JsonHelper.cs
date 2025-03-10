using HotChocolate.Data.Filters;
using HotChocolateV14.Entities;
using System.Linq;
using System.Text.Json;

namespace HotChocolateV14.Utils;

public static class JsonHelper
{
    public static List<AvailableField> ExtractAvailableFields(string json, string entityName)
    {
        var result = new List<AvailableField>();

        using JsonDocument doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty(entityName, out JsonElement entity))
        {
            foreach (var prop in entity.EnumerateObject())
            {
                string type = ExtractType(prop.Value);
                result.Add(new AvailableField { Name = prop.Name, Type = type });
            }
        }

        return result;
    }

    public static List<AvailableOperator> ExtractAvailableOperators(string json, string filterInputName, string field)
    {
        var result = new List<AvailableOperator>();
        List<string> ignoreOperators = ["and", "or"];

        using JsonDocument doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty(filterInputName, out JsonElement entityInputFilter))
        {
            if(entityInputFilter.TryGetProperty(field, out JsonElement fieldInputFilter))
            {
                if (doc.RootElement.TryGetProperty(fieldInputFilter.ToString(), out JsonElement operationInputFilter))
                {
                    foreach (var filterProp in operationInputFilter.EnumerateObject())
                    {
                        if(!ignoreOperators.Contains(filterProp.Name))
                        result.Add(new AvailableOperator { Name = filterProp.Name });
                    }
                }                    
            }
        }

        return result;
    }

    private static string ExtractType(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            return element.GetString() ?? "Unknown";
        }
        else if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("type", out JsonElement typeElement))
        {
            return typeElement.GetString() ?? "Unknown";
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            return "[ " + string.Join(", ", element.EnumerateArray().Select(e => e.GetString() ?? "Unknown")) + " ]";
        }

        return "Unknown";
    }
}
