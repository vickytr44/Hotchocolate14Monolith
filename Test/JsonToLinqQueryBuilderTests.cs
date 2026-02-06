using HotChocolateV14.Entities;
using HotChocolateV14.JsonToLinqConverter;
using HotChocolateV14.Test.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Xunit;

namespace HotChocolateV14.Test;

public class JsonToLinqQueryBuilderTests : IClassFixture<DbContextFixture>
{
    private readonly ApplicationContext _dbContext;

    public JsonToLinqQueryBuilderTests(DbContextFixture fixture)
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

    //// one customer --> many bills
    //[Fact]
    //public void _3_Build_WithRelatedTables_1_N_GeneratesProjectedSql()
    //{
    //    // Arrange
    //    var json = """
    //    {
    //      "MainTable": "customer",
    //      "SelectedFields": ["id", "age"],
    //      "RelatedTables": [
    //        { "Entity": "bill", "SelectedFields": ["id", "amount"] }
    //      ]
    //    }
    //    """;

    //    //SELECT "c"."Id", "c"."Age", "b"."Id", "b"."Amount"
    //    //FROM "customers" AS "c"
    //    //LEFT JOIN "bills" AS "b" ON "c"."Id" = "b"."CustomerId"
    //    //ORDER BY "c"."Id"
    //    const string expected = "SELECT \"c\".\"Id\", \"c\".\"Age\", \"b\".\"Id\", \"b\".\"Amount\"\r\nFROM \"customers\" AS \"c\"\r\nLEFT JOIN \"bills\" AS \"b\" ON \"c\".\"Id\" = \"b\".\"CustomerId\"\r\nORDER BY \"c\".\"Id\"";

    //    var request = JsonSerializer.Deserialize<QueryRequest>(json)!;
    //    var builder = new JsonToLinqQueryBuilder(_dbContext);

    //    // Act
    //    IQueryable query = builder.Build(request);

    //    var sql = query.ToQueryString();

    //    // Assert
    //    Assert.NotNull(query);
    //    Assert.IsAssignableFrom<IQueryable>(query);

    //    Assert.Equal(SqlNormalizer.NormalizeSql(expected), SqlNormalizer.NormalizeSql(sql));
    //}

    // one bill --> one customer
    [Fact]
    public void Build_WithRelatedTables_1_1_GeneratesProjectedSql()
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

        //SELECT "c"."Id", "c"."Name", "b"."Id", "b"."Amount"
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

    [Fact]
    public void Build_WithMultipleRelatedTables_GeneratesProjectedSql()
    {
        // Arrange
        var json = """
        {
          "MainTable": "customer",
          "SelectedFields": ["id", "age"],
          "RelatedTables": [
            { "Entity": "bill", "SelectedFields": ["id", "amount"] },
            { "Entity": "account", "SelectedFields": ["id", "IsActive"] }
          ]
        }
        """;

        //SELECT "c"."Id", "c"."Age", "b"."Id", "b"."Amount", "a"."Id", "a"."IsActive"
        //FROM "customers" AS "c"
        //LEFT JOIN "bills" AS "b" ON "c"."Id" = "b"."CustomerId"
        //LEFT JOIN "accounts" AS "a" ON "c"."Id" = "a"."CustomerId"
        //ORDER BY "c"."Id", "b"."Id"
        const string expected = "SELECT \"c\".\"Id\", \"c\".\"Age\", \"b\".\"Id\", \"b\".\"Amount\", \"a\".\"Id\", \"a\".\"IsActive\"\r\nFROM \"customers\" AS \"c\"\r\nLEFT JOIN \"bills\" AS \"b\" ON \"c\".\"Id\" = \"b\".\"CustomerId\"\r\nLEFT JOIN \"accounts\" AS \"a\" ON \"c\".\"Id\" = \"a\".\"CustomerId\"\r\nORDER BY \"c\".\"Id\", \"b\".\"Id\"";

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
