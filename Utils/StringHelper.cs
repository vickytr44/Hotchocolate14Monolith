namespace HotChocolateV14.Utils;

public static class StringHelper
{    
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }
}
