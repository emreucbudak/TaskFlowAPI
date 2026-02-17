using Chat.Infrastructure.Extensions;
using Chat.Infrastructure.Hubs;
using Chat.Persistence.Extensions;
using FlashMediator;
using FluentValidation;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Infrastructure.Extensions;
using Identity.Infrastructure.Services;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Notification.Infrastructure.Extensions;
using Notification.Persistence.Extensions;
using ProjectManagement.Infrastructure.Extensions;
using Report.Infrastructure.Extensions;
using Report.Persistence.Extensions;
using Serilog;
using Stats.Persistence.Extensions;
using TaskFlow.BuildingBlocks.Extensions;
using TaskFlow.BuildingBlocks.RabbitMQ.Contracts;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;
using Tenant.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Taskflow.Presentation.Authorization;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/logs/logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog(logger);
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddScoped<IGroupValidationService, GroupValidationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddConfigureTenant(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddChatPersistence(builder.Configuration);
builder.Services.AddChatInfrastructure();
builder.Services.AddProjectManagementInfrastructure(builder.Configuration);
builder.Services.AddNotificationPersistence(builder.Configuration);
builder.Services.AddNotificationInfrastructure();
builder.Services.AddReportModule(builder.Configuration);
builder.Services.AddReportInfrastructure(builder.Configuration);
builder.Services.RegisterCapExtensions(builder.Configuration,"taskflow.host");
builder.Services.AddStatsModule(builder.Configuration);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFlashMediator(typeof(Program).Assembly);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWTSecretKey:SecretKey"])),
        ClockSkew = TimeSpan.Zero
    };  
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SubscriptionCheck", policy => policy.Requirements.Add(new SubscriptionRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, SubscriptionHandler>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Taskflow_";
});
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TaskFlow.BuildingBlocks.Behaviors.RedisCacheBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TaskFlow.BuildingBlocks.Behaviors.LimitBehavior<,>));
builder.Services.AddIdentity<User, Identity.Domain.Entities.Roles>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<IdentityManagementDbContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHubs>("/chatHub");

app.Run();