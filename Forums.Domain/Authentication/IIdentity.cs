﻿namespace Forums.Domain.Authentication;

public interface IIdentity
{
    Guid UserId { get; }

    Guid SessionId { get; }
}