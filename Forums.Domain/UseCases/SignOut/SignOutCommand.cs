﻿using Forums.Domain.Monitoring;
using MediatR;

namespace Forums.Domain.UseCases.SignOut;

public record SignOutCommand 
    : IRequest, IMonitoredRequest
{
    private const string COUNTER_NAME = "account.signedout";

    public void MonitorSuccess(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(true));

    public void MonitorFailure(DomainMetrics metrics) =>
        metrics.IncrementCount(COUNTER_NAME, 1, DomainMetrics.ResultTags(false));
}