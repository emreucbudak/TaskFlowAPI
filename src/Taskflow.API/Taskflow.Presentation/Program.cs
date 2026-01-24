using FlashMediator;
using FluentValidation;
using Serilog;
using TaskFlow.BuildingBlocks.RabbitMQ.Contracts;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;
using Tenant.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("/logs/logs.txt",rollingInterval:RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog(logger);
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddConfigureTenant(builder.Configuration);
builder.Services.AddFlashMediator(
    typeof(Program).Assembly);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
