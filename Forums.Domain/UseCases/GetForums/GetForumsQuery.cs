using Forums.Domain.Models;
using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.GetForums;

public record GetForumsQuery 
    : IRequest<IEnumerable<Forum>>, IMonitoredRequest
{
    private const string COUNTER_NAME = "forums.fetched";

    public void MonitorSuccess(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true));

    public void MonitorFailure(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}