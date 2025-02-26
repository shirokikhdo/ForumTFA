using Forums.Domain.Authentication;
using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.SignOn;

public record SignOnCommand(string Login, string Password) 
    : IRequest<IIdentity>, IMonitoredRequest
{
    private const string COUNTER_NAME = "account.signedon";

    public void MonitorSuccess(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true));

    public void MonitorFailure(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}