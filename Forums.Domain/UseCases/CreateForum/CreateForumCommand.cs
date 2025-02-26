using Forums.Domain.Models;
using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.CreateForum;

public record CreateForumCommand(string Title) 
    : IRequest<Forum>, IMonitoredRequest
{
    private const string COUNTER_NAME = "forums.created";

    public void MonitorSuccess(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true));

    public void MonitorFailure(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}