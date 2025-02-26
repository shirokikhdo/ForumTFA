using Forums.API.Authentication;
using Forums.API.Models;
using Forums.Domain.UseCases.SignIn;
using Forums.Domain.UseCases.SignOn;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Forums.API.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly ISender _mediator;

    public AccountController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SignOn(
        [FromBody] SignOn request,
        CancellationToken cancellationToken)
    {
        var command = new SignOnCommand(request.Login, request.Password);
        var identity = await _mediator.Send(command, cancellationToken);
        return Ok(identity);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(
        [FromBody] SignIn request,
        [FromServices] IAuthTokenStorage tokenStorage,
        CancellationToken cancellationToken)
    {
        var command = new SignInCommand(request.Login, request.Password);
        var (identity, token) = await _mediator.Send(command, cancellationToken);
        tokenStorage.Store(HttpContext, token);
        return Ok(identity);
    }
}