using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HotChocolateV14.Utils
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var enumValues = Enum.GetNames(context.Type);
                schema.Enum.Clear();
                
                foreach (var enumName in enumValues)
                {
                    schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiString(enumName));
                }

                schema.Type = "string";
                schema.Format = null;
            }
        }
    }
}
