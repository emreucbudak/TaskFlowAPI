using Chat.Persistence.Extensions;
using FlashMediator;
using FluentValidation;
using Identity.Infrastructure.Extensions;
using Notification.Persistence.Extensions;
using ProjectManagement.Infrastructure.Extensions;
using Report.Persistence.Extensions;
using Serilog;
using Stats.Persistence.Extensions;
using TaskFlow.BuildingBlocks.RabbitMQ.Contracts;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;
using Tenant.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/logs/logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog(logger);
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddConfigureTenant(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddChatPersistence(builder.Configuration);
builder.Services.AddProjectManagementInfrastructure(builder.Configuration);
builder.Services.AddNotificationPersistence(builder.Configuration);
builder.Services.AddReportModule(builder.Configuration);
builder.Services.AddStatsModule(builder.Configuration);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFlashMediator(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();