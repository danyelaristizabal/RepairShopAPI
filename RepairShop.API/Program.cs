using MassTransit;
using Microsoft.OpenApi.Models;
using Notifications.Infrastructure.Extensions;
using RepairShop.Common.Domain.Enums;
using RepairShop.Common.Infrastructure.Cache.DistributedCaching;
using RepairShop.Common.Infrastructure.Database.MongoDB;
using RepairShop.Infrastructure.MassTransit;
using System.Text.Json.Serialization;
using Tickets.Infrastructure.Extensions;
using Users.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

List<Action<ConfigurationManager, IRiderRegistrationConfigurator>> messageBusActors = new();
List<Action<ConfigurationManager, IRiderRegistrationContext, IKafkaFactoryConfigurator>> addTopics = new();

builder.AddMongo();
builder.AddRedis();
builder.ConfigureNotificationsModuleIfNeeded(messageBusActors, addTopics);
builder.ConfigureUsersModuleIfNeeded(messageBusActors, addTopics);
builder.ConfigureTicketsModuleIfNeeded(messageBusActors, addTopics);
await builder.AddMassTransitWithKafkaAsync(messageBusActors, addTopics);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging((loggingBuilder) => loggingBuilder
        .SetMinimumLevel(LogLevel.Trace)
        .AddConsole()
        );

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumEnumerableConverter<NotificationType>());
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RepairShop.API", Version = "v1" });
    c.UseAllOfToExtendReferenceSchemas();
});


var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();