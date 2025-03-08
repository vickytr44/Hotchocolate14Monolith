using System;
using System.Text.RegularExpressions;

namespace SchemaToFileExtractor;

public static class GraphQLSchemaCleaner
{
    public static string CleanGraphQLSchema(string schemaText)
    {
        if (string.IsNullOrWhiteSpace(schemaText))
            return string.Empty;

        // Remove all comments (single-line and multi-line)
        // Remove all comments (single-line and multi-line)
        schemaText = Regex.Replace(schemaText, """".*?"""", "", RegexOptions.Singleline);
        schemaText = Regex.Replace(schemaText, "\"[^\"]*\"", "");

        // Remove all @cost directives (regardless of the weight value)
        schemaText = Regex.Replace(schemaText, @"@cost\([^)]*\)", "");

        // Remove @listSize directives
        schemaText = Regex.Replace(schemaText, @"@listSize\([^)]*\)", "");

        // Remove directive @cost block completely
        schemaText = Regex.Replace(schemaText, @"directive\s+@cost\s*\([^)]*\)\s*on\s*[A-Z_| ]+\n?", "");

        // Remove any remaining orphaned directive lines
        schemaText = Regex.Replace(schemaText, @"directive\s+[^\n]+\n?", "");

        // Clean up extra spaces and empty lines
        schemaText = Regex.Replace(schemaText, @"\n\s*\n", "\n").Trim();

        return schemaText;
    }
}
