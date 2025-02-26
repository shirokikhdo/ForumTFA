using Forums.Domain.Models;
using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.GetTopics;

public record GetTopicsQuery(Guid ForumId, int Skip, int Take) 
    : IRequest<(IEnumerable<Topic> resources, int totalCount)>, IMonitoredRequest
{
    private const string COUNTER_NAME = "topics.fetched";

    public void MonitorSuccess(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true));

    public void MonitorFailure(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}