using HotChocolateV14.Entities;
using HotChocolateV14.JsonToLinqConverter;
using HotChocolateV14.Test.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Xunit;

namespace HotChocolateV14.Test;

public class WhereConditionJsonToLinqQueryBuilderTests : IClassFixture<DbContextFixture>
{
    private readonly ApplicationContext _dbContext;

    public WhereConditionJsonToLinqQueryBuilderTests(DbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public void _1_Build_WithMainTableCustomer_AndConditions_ReturnsCustomerQueryable()
    {
        // Arrange
        var json = """
        {
          "MainTable": "customer",
          "AndConditions": [
            {
              "Entity": "customer",
              "Field": "age",
              "Operation": "gt",
              "Value": 18
            }
          ]
        }
        """;

        //SELECT "c"."Id", "c"."Age", "c"."IdentityNumber", "c"."Name"
        //FROM "customers" AS "c"
        //WHERE "c"."Age" > 18
        const string expected = "SELECT \"c\".\"Id\", \"c\".\"Age\", \"c\".\"IdentityNumber\", \"c\".\"Name\"\r\nFROM \"customers\" AS \"c\"\r\nWHERE \"c\".\"Age\" > 18";

        var request = JsonSerializer.Deserialize<QueryRequest>(json)!;
        var builder = new JsonToLinqQueryBuilder(_dbContext);

        // Act
        IQueryable query = builder.Build(request);

        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(query);
        Assert.IsAssignableFrom<IQueryable>(query);
        Assert.Equal(typeof(Customer), query.ElementType);

        Assert.Equal(SqlNormalizer.NormalizeSql(expected), SqlNormalizer.NormalizeSql(sql));
    }

    [Fact]
    public void _2_Build_WithSelectedFields_AndConditions_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "customer",
          "AndConditions": [
            {
              "Entity": "customer",
              "Field": "age",
              "Operation": "gt",
              "Value": 18
            }
          ],
          "SelectedFields": ["id", "age"]
        }
        """;

        //SELECT "c"."Id", "c"."Age"
        //FROM "customers" AS "c"
        //WHERE "c"."Age" > 18
        const string expected = "SELECT \"c\".\"Id\", \"c\".\"Age\"\r\nFROM \"customers\" AS \"c\"\r\nWHERE \"c\".\"Age\" > 18";

        var request = JsonSerializer.Deserialize<QueryRequest>(json)!;
        var builder = new JsonToLinqQueryBuilder(_dbContext);

        // Act
        IQueryable query = builder.Build(request);

        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(query);
        Assert.IsAssignableFrom<IQueryable>(query);

        Assert.Equal(SqlNormalizer.NormalizeSql(expected), SqlNormalizer.NormalizeSql(sql));
    }

    // many bill --> one customer
    [Fact]
    public void _3_Build_WithRelatedTables_AndConditions_N_1_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "bill",
          "SelectedFields": ["id", "amount"],
          "AndConditions": [
              {
                "Entity": "bill",
                "Field": "amount",
                "Operation": "gt",
                "Value": 100
              },
              {
                "Entity": "customer",
                "Field": "name",
                "Operation": "eq",
                "Value": "Alice"
              }
            ],
          "RelatedTables": [
            { "Entity": "customer", "SelectedFields": ["id", "name"] }
          ]
        }
        """;

        //SELECT "b"."Id", "b"."Amount", "c"."Id", "c"."Name"
        //FROM "bills" AS "b"
        //INNER JOIN "customers" AS "c" ON "b"."CustomerId" = "c"."Id"
        //WHERE ef_compare("b"."Amount", '100.0') > 0 AND "c"."Name" = 'Alice'
        const string expected = "SELECT \"b\".\"Id\", \"b\".\"Amount\", \"c\".\"Id\", \"c\".\"Name\"\r\nFROM \"bills\" AS \"b\"\r\nINNER JOIN \"customers\" AS \"c\" ON \"b\".\"CustomerId\" = \"c\".\"Id\"\r\nWHERE ef_compare(\"b\".\"Amount\", '100.0') > 0 AND \"c\".\"Name\" = 'Alice'";

        var request = JsonSerializer.Deserialize<QueryRequest>(json)!;
        var builder = new JsonToLinqQueryBuilder(_dbContext);

        // Act
        IQueryable query = builder.Build(request);

        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(query);
        Assert.IsAssignableFrom<IQueryable>(query);

        Assert.Equal(SqlNormalizer.NormalizeSql(expected), SqlNormalizer.NormalizeSql(sql));
    }

    [Fact]
    public void _4_Build_WithMultipleRelatedTables_AndConditions_N_1_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "bill",
          "SelectedFields": ["id", "amount"],
          "AndConditions": [
                {
                "Entity": "bill",
                "Field": "amount",
                "Operation": "gt",
                "Value": 100
                },
                {
                "Entity": "customer",
                "Field": "name",
                "Operation": "eq",
                "Value": "Alice"
                },
                {
                "Entity": "account",
                "Field": "isActive",
                "Operation": "eq",
                "Value": "true"
                }
            ],
          "RelatedTables": [
            { "Entity": "customer", "SelectedFields": ["id", "name"] },
            { "Entity": "account", "SelectedFields": ["id", "number", "isActive"] }
          ]
        }
        """;

        //SELECT "b"."Id", "b"."Amount", "c"."Id", "c"."Name", "a"."Id", "a"."Number", "a"."IsActive"
        //FROM "bills" AS "b"
        //INNER JOIN "customers" AS "c" ON "b"."CustomerId" = "c"."Id"
        //INNER JOIN "accounts" AS "a" ON "b"."AccountId" = "a"."Id"
        //WHERE ef_compare("b"."Amount", '100.0') > 0 AND "c"."Name" = 'Alice' AND "a"."IsActive"
        const string expected = "SELECT \"b\".\"Id\", \"b\".\"Amount\", \"c\".\"Id\", \"c\".\"Name\", \"a\".\"Id\", \"a\".\"Number\", \"a\".\"IsActive\"\r\nFROM \"bills\" AS \"b\"\r\nINNER JOIN \"customers\" AS \"c\" ON \"b\".\"CustomerId\" = \"c\".\"Id\"\r\nINNER JOIN \"accounts\" AS \"a\" ON \"b\".\"AccountId\" = \"a\".\"Id\"\r\nWHERE ef_compare(\"b\".\"Amount\", '100.0') > 0 AND \"c\".\"Name\" = 'Alice' AND \"a\".\"IsActive\"";

        var request = JsonSerializer.Deserialize<QueryRequest>(json)!;
        var builder = new JsonToLinqQueryBuilder(_dbContext);

        // Act
        IQueryable query = builder.Build(request);

        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(query);
        Assert.IsAssignableFrom<IQueryable>(query);

        Assert.Equal(SqlNormalizer.NormalizeSql(expected), SqlNormalizer.NormalizeSql(sql));
    }
}
