using Forum.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ForumController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(string[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] ForumDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var forumTitles = await dbContext.Forums
            .Select(f => f.Title)
            .ToArrayAsync(cancellationToken);
        return Ok(forumTitles);
    }
}