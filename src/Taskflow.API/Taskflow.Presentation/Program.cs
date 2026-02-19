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
using Microsoft.AspNetCore.RateLimiting;
using Notification.Infrastructure.Extensions;
using Notification.Infrastructure.Hubs;
using Notification.Persistence.Extensions;
using ProjectManagement.Infrastructure.Extensions;
using Report.Infrastructure.Extensions;
using Report.Persistence.Extensions;
using Serilog;
using Stats.Persistence.Extensions;
using System.Threading.RateLimiting;
using TaskFlow.BuildingBlocks.Extensions;
using TaskFlow.BuildingBlocks.RabbitMQ.Contracts;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;
using Tenant.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Taskflow.Presentation.Authorization;
using Taskflow.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

var postgresHost = builder.Configuration["Postgres:Host"] ?? "localhost";
var postgresPort = builder.Configuration["Postgres:Port"] ?? "5432";
var postgresDatabase = builder.Configuration["Postgres:Database"] ?? "TaskFlowDb";
var postgresUser = builder.Configuration["postgres_user"];
var postgresPassword = builder.Configuration["postgres_password"];
if (!string.IsNullOrWhiteSpace(postgresUser) && !string.IsNullOrWhiteSpace(postgresPassword))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] =
        $"Host={postgresHost};Port={postgresPort};Database={postgresDatabase};Username={postgresUser};Password={postgresPassword}";
}

var redisHost = builder.Configuration["Redis:Host"] ?? "localhost";
var redisPort = builder.Configuration["Redis:Port"] ?? "6379";
var redisPassword = builder.Configuration["redis_password"];
builder.Configuration["ConnectionStrings:Redis"] =
    string.IsNullOrWhiteSpace(redisPassword)
        ? $"{redisHost}:{redisPort}"
        : $"{redisHost}:{redisPort},password={redisPassword}";

var rabbitMqUser = builder.Configuration["rabbitmq_user"];
var rabbitMqPassword = builder.Configuration["rabbitmq_password"];
if (!string.IsNullOrWhiteSpace(rabbitMqUser))
{
    builder.Configuration["RabbitMQ:UserName"] = rabbitMqUser;
}
if (!string.IsNullOrWhiteSpace(rabbitMqPassword))
{
    builder.Configuration["RabbitMQ:Password"] = rabbitMqPassword;
}

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/logs/logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog(logger);
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddScoped<IGroupValidationService, GroupValidationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddProblemDetails();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            }));
});
builder.Services.AddExceptionHandler<AuthExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<SubscriptionLimitExceededExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
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
builder.Services.AddFlashMediator(
    typeof(Program).Assembly,
    typeof(Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesHandler).Assembly);
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
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CompanyPolicy", policy => policy.RequireRole("Company"));
    options.AddPolicy("WorkerPolicy", policy => policy.RequireRole("Worker"));
    options.AddPolicy("CompanyOrWorkerPolicy", policy => policy.RequireRole("Company", "Worker"));
    options.AddPolicy("AdminOrCompanyPolicy", policy => policy.RequireRole("Admin", "Company"));
    options.AddPolicy("SubscriptionCheck", policy => policy.Requirements.Add(new SubscriptionRequirement()));
    options.AddPolicy("SubscribedCompanyPolicy", policy =>
    {
        policy.RequireRole("Company");
        policy.Requirements.Add(new SubscriptionRequirement());
    });
    options.AddPolicy("SubscribedWorkerPolicy", policy =>
    {
        policy.RequireRole("Worker");
        policy.Requirements.Add(new SubscriptionRequirement());
    });
    options.AddPolicy("SubscribedCompanyOrWorkerPolicy", policy =>
    {
        policy.RequireRole("Company", "Worker");
        policy.Requirements.Add(new SubscriptionRequirement());
    });
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

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHubs>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
