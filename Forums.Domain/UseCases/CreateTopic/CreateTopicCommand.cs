using Forums.Domain.Models;
using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.CreateTopic;

public record CreateTopicCommand(Guid ForumId, string Title) 
    : IRequest<Topic>, IMonitoredRequest
{
    private const string COUNTER_NAME = "topics.created";

    public void MonitorSuccess(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true));

    public void MonitorFailure(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}