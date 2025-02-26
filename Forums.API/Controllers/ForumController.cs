using AutoMapper;
using Forums.API.Models;
using Forums.Domain.UseCases.CreateForum;
using Forums.Domain.UseCases.CreateTopic;
using Forums.Domain.UseCases.GetForums;
using Forums.Domain.UseCases.GetTopics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Forum = Forums.API.Models.Forum;

namespace Forums.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public ForumController(
        ISender mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(201, Type = typeof(Forum))]
    public async Task<IActionResult> CreateForum(
        [FromBody] CreateForum request,
        CancellationToken cancellationToken)
    {
        var command = new CreateForumCommand(request.Title);
        var forum = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtRoute(nameof(GetForums), _mapper.Map<Forum>(forum));
    }

    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(Forum[]))]
    public async Task<IActionResult> GetForums(CancellationToken cancellationToken)
    {
        var query = new GetForumsQuery();
        var forums = await _mediator.Send(query, cancellationToken);

        return Ok(forums.Select(_mapper.Map<Forum>));
    }

    [HttpPost("{forumId:guid}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(410)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTopicCommand(forumId, request.Title);
        var topic = await _mediator.Send(command, cancellationToken);

        return CreatedAtRoute(nameof(GetForums), _mapper.Map<Topic>(topic));
    }

    [HttpGet("{forumId:guid}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(410)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetTopics(
        [FromRoute] Guid forumId,
        [FromQuery] int skip,
        [FromQuery] int take,
        CancellationToken cancellationToken)
    {
        var query = new GetTopicsQuery(forumId, skip, take);
        var (resources, totalCount) = await _mediator.Send(query, cancellationToken);

        return Ok(new { resources = resources.Select(_mapper.Map<Topic>), totalCount });
    }
}