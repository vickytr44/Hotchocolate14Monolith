using HotChocolateV14.Entities;
using HotChocolateV14.JsonToLinqConverter;
using HotChocolateV14.Test.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Xunit;

namespace HotChocolateV14.Test;

public class MainAndRelatedTableJsonToLinqQueryBuilderTests : IClassFixture<DbContextFixture>
{
    private readonly ApplicationContext _dbContext;

    public MainAndRelatedTableJsonToLinqQueryBuilderTests(DbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public void _1_Build_WithMainTableCustomer_ReturnsCustomerQueryable()
    {
        // Arrange
        var json = """
        {
          "MainTable": "customer"
        }
        """;

        //SELECT "c"."Id", "c"."Age", "c"."IdentityNumber", "c"."Name"
        //FROM "customers" AS "c"
        const string expected = "select \"c\".\"id\", \"c\".\"age\", \"c\".\"identitynumber\", \"c\".\"name\" from \"customers\" as \"c\"";

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
    public void _2_Build_WithSelectedFields_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "customer",
          "SelectedFields": ["id", "age"]
        }
        """;

        //SELECT "c"."Id", "c"."Age", "c"."IdentityNumber", "c"."Name"
        //FROM "customers" AS "c"
        const string expected = "select \"c\".\"id\", \"c\".\"age\" from \"customers\" as \"c\"";

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
    public void _3_Build_WithRelatedTables_N_1_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "bill",
          "SelectedFields": ["id", "amount"],
          "RelatedTables": [
            { "Entity": "customer", "SelectedFields": ["id", "name"] }
          ]
        }
        """;

        //SELECT "b"."Id", "b"."Amount", "c"."Id", "c"."Name"
        //FROM "bills" AS "b"
        //INNER JOIN "customers" AS "c" ON "b"."CustomerId" = "c"."Id"
        const string expected = "SELECT \"b\".\"Id\", \"b\".\"Amount\", \"c\".\"Id\", \"c\".\"Name\"\r\nFROM \"bills\" AS \"b\"\r\nINNER JOIN \"customers\" AS \"c\" ON \"b\".\"CustomerId\" = \"c\".\"Id\"";

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
    public void _4_Build_WithMultipleRelatedTables_N_1_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "bill",
          "SelectedFields": ["id", "amount"],
          "RelatedTables": [
            { "Entity": "customer", "SelectedFields": ["id", "name"] },
            { "Entity": "account", "SelectedFields": ["id", "number"] }
          ]
        }
        """;

        //SELECT "b"."Id", "b"."Amount", "c"."Id", "c"."Name", "a"."Id", "a"."Number"
        //FROM "bills" AS "b"
        //INNER JOIN "customers" AS "c" ON "b"."CustomerId" = "c"."Id"
        //INNER JOIN "accounts" AS "a" ON "b"."AccountId" = "a"."Id"
        const string expected = "SELECT \"b\".\"Id\", \"b\".\"Amount\", \"c\".\"Id\", \"c\".\"Name\", \"a\".\"Id\", \"a\".\"Number\"\r\nFROM \"bills\" AS \"b\"\r\nINNER JOIN \"customers\" AS \"c\" ON \"b\".\"CustomerId\" = \"c\".\"Id\"\r\nINNER JOIN \"accounts\" AS \"a\" ON \"b\".\"AccountId\" = \"a\".\"Id\"";

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

    // one customer --> many bills
    [Fact]
    public void _5_Build_WithRelatedTables_1_N_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "customer",
          "SelectedFields": ["id", "age"],
          "RelatedTables": [
            { "Entity": "bill", "SelectedFields": ["id", "amount"] }
          ]
        }
        """;

        //SELECT "c"."Id", "c"."Age", "b"."Id", "b"."Amount"
        //FROM "customers" AS "c"
        //LEFT JOIN "bills" AS "b" ON "c"."Id" = "b"."CustomerId"
        //ORDER BY "c"."Id"
        const string expected = "SELECT \"c\".\"Id\", \"c\".\"Age\", \"b\".\"Id\", \"b\".\"Amount\"\r\nFROM \"customers\" AS \"c\"\r\nLEFT JOIN \"bills\" AS \"b\" ON \"c\".\"Id\" = \"b\".\"CustomerId\"\r\nORDER BY \"c\".\"Id\"";

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
