using Forums.API.Middlewares;
using Forums.Domain.DependencyInjection;
using Forums.Storage.DependencyInjection;
using Forums.API.Authentication;
using Forums.API.DependencyInjection;
using Forums.Domain.Authentication;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("PostgresConnectionString");

builder.Services.AddApiLogging(builder.Configuration, builder.Environment);
builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication").Bind);
builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();

builder.Services
    .AddForumDomain()
    .AddForumStorage(connectionString);
builder.Services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

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

public partial class Program
{

}