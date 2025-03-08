using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using HotChocolateV14.Utils;

namespace HotChocolateV14.Queries;

[QueryType]
public class AvailableOperatorQuery
{
    [UseFiltering]
    [UseSorting]
    public IEnumerable<AvailableOperator> GetAvailableOperators(string entity, string field)
    {
        var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>(entity);

        var filePath = @"C:\HotChocolateGraphQL14\HotChocolateV14\SchemaToFileExtractor\Files\inputs.json";
        string json = File.ReadAllText(filePath);

        return entityEnum switch
        {
            Entity.Customer => GetOperatorsFor<Customer>(json, Entity.Customer.ToString(), field),
            Entity.Bill => GetOperatorsFor<Bill>(json, Entity.Bill.ToString(), field),
            Entity.Account => GetOperatorsFor<Account>(json, Entity.Account.ToString(), field),
            _ => [],
        };
    }

    private static List<AvailableOperator> GetOperatorsFor<T>(string json, string entity, string field)
    {
        var operators = JsonHelper.ExtractAvailableOperators(json, entity + "FilterInput", field.ToCamelCase());

        return operators;
    }
}
