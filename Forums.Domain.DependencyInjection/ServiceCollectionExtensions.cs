using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Models;
using Forums.Domain.UseCases.CreateTopic;
using Forums.Domain.UseCases.GetForums;
using Forums.Domain.UseCases.GetTopics;

namespace Forums.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services
            .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
            .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
            .AddScoped<IGetTopicsUseCase, GetTopicsUseCase>()
            .AddScoped<IIntentionResolver, TopicIntentionResolver>();

        services
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIdentityProvider, IdentityProvider>();

        services.AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);

        services.AddMemoryCache();

        return services;
    }
}