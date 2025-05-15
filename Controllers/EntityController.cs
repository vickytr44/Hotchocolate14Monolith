using Microsoft.AspNetCore.Mvc;
using HotChocolateV14.Constants;

namespace HotChocolateV14.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetEntities()
        {
            var entities = Enum.GetValues(typeof(Entity))
                .Cast<Entity>()
                .Select(e => e.ToString().ToLower() + "s")
                .ToList();

            return Ok(entities);
        }
    }
}