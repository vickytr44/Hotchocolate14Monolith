using Microsoft.AspNetCore.Mvc;
using HotChocolateV14.Constants;
using System.Text.Json;
using Path = System.IO.Path;
using Swashbuckle.AspNetCore.Annotations;

namespace HotChocolateV14.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly string _typesJsonPath;

        public EntityController(IWebHostEnvironment env)
        {
            // Use the content root path to ensure we get the correct project directory
            _typesJsonPath = Path.Combine(env.ContentRootPath, 
                "SchemaToFileExtractor", 
                "Files", 
                "Jsons", 
                "types.json");
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetEntities()
        {
            var entities = Enum.GetValues(typeof(Entity))
                .Cast<Entity>()
                .Select(e => e.ToString().ToLower() + "s")
                .ToList();

            return Ok(entities);
        }

        [HttpGet("{entity}/related")]
        [SwaggerOperation(
            Summary = "Get related entities",
            Description = "Gets all related entities for the specified entity type"
        )]
        public ActionResult<IEnumerable<string>> GetRelatedEntities([SwaggerParameter(Description = "The entity type")] Entity entity)
        {
            try
            {
                if (!System.IO.File.Exists(_typesJsonPath))
                {
                    return StatusCode(500, "types.json file not found");
                }

                var jsonContent = System.IO.File.ReadAllText(_typesJsonPath);
                var types = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

                if (types == null)
                {
                    return StatusCode(500, "Failed to parse types.json");
                }

                // Get entity name from enum
                var entityName = entity.ToString();
                
                if (!types.ContainsKey(entityName))
                {
                    return NotFound($"Entity '{entityName}' not found");
                }

                var entityFields = types[entityName];
                var relatedEntities = new HashSet<string>();

                foreach (var field in entityFields)
                {
                    // Check if the field type is a reference to another entity (not a primitive type or array)
                    var fieldType = field.Value.TrimEnd('!'); // Remove non-null indicator
                    if (fieldType.StartsWith("["))
                    {
                        // Handle array types, extract the entity name
                        fieldType = fieldType.Trim('[', ']');
                        fieldType = fieldType.TrimEnd('!');
                    }

                    // If the field type exists in our types dictionary, it's a related entity
                    if (types.ContainsKey(fieldType))
                    {
                        relatedEntities.Add(fieldType.ToLower());
                    }
                }

                return Ok(relatedEntities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting related entities: {ex.Message}");
            }
        }

        [HttpGet("{entity}/fields")]
        [SwaggerOperation(
            Summary = "Get entity fields",
            Description = "Gets all primitive fields (non-entity fields) for the specified entity type"
        )]
        public ActionResult<IEnumerable<string>> GetEntityFields([SwaggerParameter(Description = "The entity type")] Entity entity)
        {
            try
            {
                if (!System.IO.File.Exists(_typesJsonPath))
                {
                    return StatusCode(500, "types.json file not found");
                }

                var jsonContent = System.IO.File.ReadAllText(_typesJsonPath);
                var types = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

                if (types == null)
                {
                    return StatusCode(500, "Failed to parse types.json");
                }

                var entityName = entity.ToString();
                
                if (!types.ContainsKey(entityName))
                {
                    return NotFound($"Entity '{entityName}' not found");
                }

                var entityFields = types[entityName];
                var primitiveFields = new List<string>();

                foreach (var field in entityFields)
                {
                    var fieldType = field.Value.TrimEnd('!'); // Remove non-null indicator
                    if (fieldType.StartsWith("["))
                    {
                        // Handle array types, extract the type
                        fieldType = fieldType.Trim('[', ']');
                        fieldType = fieldType.TrimEnd('!');
                    }

                    // If the field type doesn't exist in our types dictionary, it's a primitive type
                    if (!types.ContainsKey(fieldType))
                    {
                        primitiveFields.Add(field.Key.ToLower());
                    }
                }

                return Ok(primitiveFields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting entity fields: {ex.Message}");
            }
        }
    }
}