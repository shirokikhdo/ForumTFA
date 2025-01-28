﻿using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.GetForums;

internal static class GetForumsStorageExtensions
{
    public static async Task ThrowIfForumNotFound(
        this IGetForumsStorage storage,
        Guid forumId,
        CancellationToken cancellationToken)
    {
        if (!await ForumExists(storage, forumId, cancellationToken))
            throw new ForumNotFoundException(forumId);
    }

    private static async Task<bool> ForumExists(
        this IGetForumsStorage storage,
        Guid forumId,
        CancellationToken cancellationToken)
    {
        var forums = await storage.GetForums(cancellationToken);
        return forums.Any(f => f.Id == forumId);
    }
}