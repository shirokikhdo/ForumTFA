using Forums.Domain.Authentication;
using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.SignIn;

public record SignInCommand(string Login, string Password) 
    : IRequest<(IIdentity identity, string token)>, IMonitoredRequest
{ 
    private const string COUNTER_NAME = "account.signedin"; 
    
    public void MonitorSuccess(DomainMetrics metrics) => 
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true)); 
    
    public void MonitorFailure(DomainMetrics metrics) => 
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}