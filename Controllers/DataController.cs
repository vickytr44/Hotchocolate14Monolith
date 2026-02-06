using HotChocolateV14.JsonToLinqConverter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotChocolateV14.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController(IJsonToLinqQueryBuilder jsonToLinqQueryBuilder) : ControllerBase
{
    [HttpPost()]
    public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request)
    {
        var query = jsonToLinqQueryBuilder.Build(request);
        return Ok(await query.Cast<Dictionary<string, object>>().ToListAsync());
    }
}
