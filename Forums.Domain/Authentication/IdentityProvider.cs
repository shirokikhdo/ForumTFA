﻿namespace Forums.Domain.Authentication;

internal class IdentityProvider : IIdentityProvider
{
    public IIdentity Current { get; set; }

    //public IIdentity Current =>
    //    new User(Guid.Parse("1B64BC92-9219-48E3-8589-9253080ED066"));
}