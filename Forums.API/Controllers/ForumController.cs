using Forums.API.Models;
using Forums.Domain.UseCases.CreateTopic;
using Forums.Domain.UseCases.GetForums;
using Microsoft.AspNetCore.Mvc;
using Forum = Forums.API.Models.Forum;

namespace Forums.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController : ControllerBase
{
    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(Forum[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] IGetForumsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var forums = await useCase.Execute(cancellationToken);
        
        return Ok(forums.Select(f => new Forum
        {
            Id = f.Id,
            Title = f.Title
        }));
    }

    [HttpPost("{forumId:guid}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(410)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        [FromServices] ICreateTopicUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new CreateTopicCommand(forumId, request.Title);
        var topic = await useCase.Execute(command, cancellationToken);
        return CreatedAtRoute(nameof(GetForums), new Topic
        {
            Id = topic.Id,
            Title = topic.Title,
            CreatedAt = topic.CreatedAt
        });
    }
}