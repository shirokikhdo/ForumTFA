using Forums.Domain.UseCases.SignOut;

namespace Forums.Storage.Storages;

internal class SignOutStorage : ISignOutStorage
{
    public Task RemoveSession(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}