using System.Reflection;
using Forums.Domain.UseCases.CreateForum;
using Forums.Domain.UseCases.CreateTopic;
using Forums.Domain.UseCases.GetForums;
using Forums.Domain.UseCases.GetTopics;
using Forums.Domain.UseCases.SignIn;
using Forums.Domain.UseCases.SignOn;
using Forums.Storage.Models;
using Forums.Storage.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forums.Storage.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumStorage(this IServiceCollection services, string dbConnectionString)
    {
        services
            .AddScoped<ICreateForumStorage, CreateForumStorage>()
            .AddScoped<IGetForumsStorage, GetForumsStorage>()
            .AddScoped<ICreateTopicStorage, CreateTopicStorage>()
            .AddScoped<IGetTopicsStorage, GetTopicsStorage>()
            .AddScoped<ISignOnStorage, SignOnStorage>()
            .AddScoped<ISignInStorage, SignInStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IMomentProvider, MomentProvider>()
            .AddDbContextPool<ForumDbContext>(options => options
                .UseNpgsql(dbConnectionString));

        services.AddMemoryCache();

        services.AddAutoMapper(config => config
            .AddMaps(Assembly.GetAssembly(typeof(ForumDbContext))));
        
        return services;
    }
}