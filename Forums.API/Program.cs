using FluentValidation;
using Forums.API.Middlewares;
using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Models;
using Forums.Domain.UseCases.CreateTopic;
using Forums.Domain.UseCases.GetForums;
using Forums.Storage;
using Forums.Storage.Storages;
using Microsoft.EntityFrameworkCore;
using Forum = Forums.Domain.Models.Forum;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("PostgresConnectionString");
builder.Services.AddDbContextPool<ForumDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IGetForumsUseCase, GetForumsUseCase>();
builder.Services.AddScoped<IGetForumsStorage, GetForumsStorage>();
builder.Services.AddScoped<ICreateTopicUseCase, CreateTopicUseCase>();
builder.Services.AddScoped<ICreateTopicStorage, CreateTopicStorage>();
builder.Services.AddScoped<IIntentionResolver, TopicIntentionResolver>();
builder.Services.AddScoped<IIntentionManager, IntentionManager>();
builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
builder.Services.AddScoped<IGuidFactory, GuidFactory>();
builder.Services.AddScoped<IMomentProvider, MomentProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<Forum>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.Run();