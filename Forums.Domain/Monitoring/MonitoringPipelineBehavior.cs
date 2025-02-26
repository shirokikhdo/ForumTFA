using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forums.Domain.Monitoring;

internal class MonitoringPipelineBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    private readonly DomainMetrics _metrics;
    private readonly ILogger<MonitoringPipelineBehavior<TRequest, TResponse>> _logger;

    public MonitoringPipelineBehavior(
        DomainMetrics metrics, 
        ILogger<MonitoringPipelineBehavior<TRequest, TResponse>> logger)
    {
        _metrics = metrics;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IMonitoredRequest monitoredRequest) 
            return await next.Invoke();

        using var activity = DomainMetrics.ActivitySource.StartActivity(
            "usecase", 
            ActivityKind.Internal, 
            default(ActivityContext));

        activity?.AddTag("forums-tfa.command", request.GetType().Name);

        try
        {
            var result = await next.Invoke();
            _logger.LogInformation("Command successfully handled {Command}", request);
            monitoredRequest.MonitorSuccess(_metrics);
            activity?.AddTag("error", false);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled error caught while handling command {Command}", request);
            monitoredRequest.MonitorFailure(_metrics);
            activity?.AddTag("error", true);
            throw;
        }
    }
}