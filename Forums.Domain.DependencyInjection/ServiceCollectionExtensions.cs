using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Models;
using Forums.Domain.UseCases.CreateForum;
using Forums.Domain.UseCases.CreateTopic;
using Forums.Domain.UseCases.GetForums;
using Forums.Domain.UseCases.GetTopics;
using Forums.Domain.UseCases.SignIn;
using Forums.Domain.UseCases.SignOn;
using Forums.Domain.UseCases.SignOut;
using Forums.Domain.Monitoring;

namespace Forums.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services
            .AddScoped<ICreateForumUseCase, CreateForumUseCase>()
            .AddScoped<IIntentionResolver, ForumIntentionResolver>()
            .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
            .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
            .AddScoped<IGetTopicsUseCase, GetTopicsUseCase>()
            .AddScoped<ISignOnUseCase, SignOnUseCase>()
            .AddScoped<ISignInUseCase, SignInUseCase>()
            .AddScoped<ISignOutUseCase, SignOutUseCase>()
            .AddScoped<IIntentionResolver, TopicIntentionResolver>();

        services
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIdentityProvider, IdentityProvider>()
            .AddScoped<IPasswordManager, PasswordManager>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ISymmetricDecryptor, AesSymmetricEncryptorDecryptor>()
            .AddScoped<ISymmetricEncryptor, AesSymmetricEncryptorDecryptor>();

        services.AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);

        services.AddSingleton<DomainMetrics>();

        return services;
    }
}