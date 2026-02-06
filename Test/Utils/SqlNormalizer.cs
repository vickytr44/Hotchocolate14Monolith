using System.Text.RegularExpressions;

namespace HotChocolateV14.Test.Utils;

public static class SqlNormalizer
{
    //\s+ matches one or more whitespace characters
    //spaces
    //tabs
    //newlines
    //Replaces any run of whitespace with a single space
    public static string NormalizeSql(string sql)
    {
        return Regex.Replace(sql, @"\s+", " ")
            .Trim()
            .ToLowerInvariant();
    }
}
