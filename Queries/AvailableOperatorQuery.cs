using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using HotChocolateV14.Utils;

namespace HotChocolateV14.Queries;

//[QueryType]
public class AvailableOperatorQuery
{
    [UseFiltering]
    [UseSorting]
    public IEnumerable<AvailableOperator> GetAvailableOperators(string entity, string field)
    {
        var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>(entity);

        var filePath = $"{FilePath.JsonFilePath}inputs.json";
        string json = File.ReadAllText(filePath);

        return GetOperatorsFor(json, entityEnum.ToString(), field);
    }

    private static List<AvailableOperator> GetOperatorsFor(string json, string entity, string field)
    {
        var operators = JsonHelper.ExtractAvailableOperators(json, entity + "FilterInput", field.ToCamelCase());

        return operators;
    }
}
