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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Notification.Infrastructure.Extensions;
using Notification.Infrastructure.Hubs;
using Notification.Infrastructure.Data.NotificationDb;
using Notification.Persistence.Extensions;
using Npgsql;
using ProjectManagement.Persistence.Data.ProjectManagementDb;
using ProjectManagement.Infrastructure.Extensions;
using Report.Infrastructure.Extensions;
using Report.Persistence.Data;
using Report.Persistence.Extensions;
using Serilog;
using Stats.Persistence.Data;
using Stats.Persistence.Extensions;
using System.Linq;
using System.Threading.RateLimiting;
using TaskFlow.BuildingBlocks.Extensions;
using TaskFlow.BuildingBlocks.Contracts.Redis;
using TaskFlow.BuildingBlocks.Interfaces;
using TaskFlow.BuildingBlocks.RabbitMQ.Contracts;
using TaskFlow.BuildingBlocks.RabbitMQ.Interface;
using Tenant.Domain.Entities;
using Tenant.Persistence.Data.TenantDb;
using Tenant.Infrastructure.Extensions;
using Chat.Persistence.Data.ChatDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Taskflow.Presentation.Authorization;
using Taskflow.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

static string? FirstNonEmpty(params string?[] values)
{
    foreach (var value in values)
    {
        var trimmed = value?.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            return trimmed;
        }
    }

    return null;
}

static string GetSecret(string secretName, params string[] envVarAliases)
{
    var dockerSecretPath = $"/run/secrets/{secretName}";
    if (File.Exists(dockerSecretPath))
    {
        return File.ReadAllText(dockerSecretPath).Trim();
    }

    var localSecretPath = Path.GetFullPath(Path.Combine("..", "..", "secrets", secretName));
    if (File.Exists(localSecretPath))
    {
        return File.ReadAllText(localSecretPath).Trim();
    }

    var secretFromEnv = Environment.GetEnvironmentVariable(secretName);
    if (!string.IsNullOrWhiteSpace(secretFromEnv))
    {
        return secretFromEnv.Trim();
    }

    foreach (var envVarAlias in envVarAliases)
    {
        var envAliasValue = Environment.GetEnvironmentVariable(envVarAlias);
        if (!string.IsNullOrWhiteSpace(envAliasValue))
        {
            return envAliasValue.Trim();
        }
    }

    Console.WriteLine($"[UYARI] Secret '{secretName}' bulunamadi! (Docker: {dockerSecretPath}, Local: {localSecretPath})");
    return string.Empty;
}

var postgresHost = FirstNonEmpty(
    builder.Configuration["Postgres:Host"],
    Environment.GetEnvironmentVariable("TF_POSTGRES_HOST"))
    ?? "localhost";

var postgresPortText = FirstNonEmpty(
    builder.Configuration["Postgres:Port"],
    Environment.GetEnvironmentVariable("TF_POSTGRES_PORT"))
    ?? "5432";

var postgresDatabase = FirstNonEmpty(
    builder.Configuration["Postgres:Database"],
    Environment.GetEnvironmentVariable("TF_POSTGRES_DATABASE"))
    ?? "TaskFlowDb";

var postgresUser = GetSecret("postgres_user", "TF_POSTGRES_USER", "POSTGRES_USER");
var postgresPassword = GetSecret("postgres_password", "TF_POSTGRES_PASSWORD", "POSTGRES_PASSWORD");

if (string.IsNullOrWhiteSpace(postgresUser) || string.IsNullOrWhiteSpace(postgresPassword))
{
    throw new InvalidOperationException("PostgreSQL secret degerleri eksik: 'postgres_user' ve 'postgres_password' zorunlu.");
}

var postgresPort = int.TryParse(postgresPortText, out var parsedPostgresPort) ? parsedPostgresPort : 5432;
var defaultConnectionBuilder = new NpgsqlConnectionStringBuilder
{
    Host = postgresHost,
    Port = postgresPort,
    Database = postgresDatabase,
    Username = postgresUser,
    Password = postgresPassword
};
builder.Configuration["ConnectionStrings:DefaultConnection"] = defaultConnectionBuilder.ConnectionString;

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

var jwtSecretKey =
    builder.Configuration["jwt_secret_key"]
    ?? Environment.GetEnvironmentVariable("TF_JWT_SECRET_KEY");
jwtSecretKey = jwtSecretKey?.Trim();
if (!string.IsNullOrWhiteSpace(jwtSecretKey))
{
    builder.Configuration["JWTSecretKey:SecretKey"] = jwtSecretKey;
}

var chatMessageEncryptionKey = builder.Configuration["chat_message_encryption_key"]?.Trim();
if (string.IsNullOrWhiteSpace(chatMessageEncryptionKey))
{
    throw new InvalidOperationException("Chat message encryption key tanimlanmamis. Docker secret 'chat_message_encryption_key' zorunlu.");
}
builder.Configuration["ChatEncryption:Key"] = chatMessageEncryptionKey;

var resolvedJwtSecretKey = builder.Configuration["JWTSecretKey:SecretKey"]?.Trim();
if (string.IsNullOrWhiteSpace(resolvedJwtSecretKey))
{
    throw new InvalidOperationException("JWT secret key tanimlanmamis. Docker secret 'jwt_secret_key' veya 'JWTSecretKey:SecretKey' gerekli.");
}

var resolvedJwtIssuer =
    builder.Configuration["JWTSecretKey:Issuer"]?.Trim()
    ?? builder.Configuration["TokenSettings:Issuer"]?.Trim()
    ?? "TaskflowAPI";

var resolvedJwtAudience =
    builder.Configuration["JWTSecretKey:Audience"]?.Trim()
    ?? builder.Configuration["TokenSettings:Audience"]?.Trim()
    ?? "TaskflowClient";

var resolvedAccessTokenExpiryMinutesRaw =
    builder.Configuration["TokenSettings:AccessTokenExpiryMinutes"]?.Trim()
    ?? builder.Configuration["JWTSecretKey:ExpiryInMinutes"]?.Trim()
    ?? "60";

if (!int.TryParse(resolvedAccessTokenExpiryMinutesRaw, out var resolvedAccessTokenExpiryMinutes) || resolvedAccessTokenExpiryMinutes <= 0)
{
    resolvedAccessTokenExpiryMinutes = 60;
}

// Normalize legacy/new config keys into a single section consumed by TokenService.
builder.Configuration["TokenSettings:SecretKey"] = resolvedJwtSecretKey;
builder.Configuration["TokenSettings:Issuer"] = resolvedJwtIssuer;
builder.Configuration["TokenSettings:Audience"] = resolvedJwtAudience;
builder.Configuration["TokenSettings:AccessTokenExpiryMinutes"] = resolvedAccessTokenExpiryMinutes.ToString();

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
    typeof(Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesHandler).Assembly,
    typeof(Identity.Application.Features.CQRS.Auth.Login.LoginCommandHandler).Assembly,
    typeof(Chat.Application.Features.CQRS.Message.Queries.GetUnreadMessageCount.GetUnreadMessageCountQueryHandler).Assembly,
    typeof(Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications.GetUserAllNotificationsQueriesHandler).Assembly,
    typeof(ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId.GetIndividualTasksByUserIdQueryHandler).Assembly,
    typeof(Report.Application.Features.CQRS.Reports.Query.GetAll.GetAllReportsQueryHandler).Assembly,
    typeof(Stats.Application.Features.CQRS.WorkerStats.Queries.GetByUserAndPeriod.GetWorkerStatsByUserAndPeriodQueryHandler).Assembly);
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
builder.Services.AddScoped<ICacheService, CacheService>();
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = resolvedJwtIssuer,
        ValidAudience = resolvedJwtAudience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(resolvedJwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IdentityManagementDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<TenantDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<ChatDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<ReportDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<ProjectManagementDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<StatsDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await EnsureSeedDemoAccountAsync(app.Services);
}
else
{
    app.UseHsts();
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

static async Task EnsureSeedDemoAccountAsync(IServiceProvider services)
{
    const string defaultCompanyName = "TaskFlow Demo Company";
    const string defaultEmail = "demo@taskflow.dev";
    const string defaultPassword = "TaskFlow!23";
    const string defaultRole = "Company";

    using var scope = services.CreateScope();
    var scopedServices = scope.ServiceProvider;
    var logger = scopedServices.GetRequiredService<ILogger<Program>>();
    var identityContext = scopedServices.GetRequiredService<IdentityManagementDbContext>();
    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
    var roleManager = scopedServices.GetRequiredService<RoleManager<Roles>>();
    var tenantContext = scopedServices.GetRequiredService<TenantDbContext>();

    if (!await identityContext.Companies.AnyAsync())
    {
        identityContext.Companies.Add(new Company(defaultCompanyName));
        await identityContext.SaveChangesAsync();
        logger.LogInformation("Varsayilan sirket '{Company}' olusturuldu.", defaultCompanyName);
    }

    var company = await identityContext.Companies.OrderBy(c => c.Id).FirstAsync();

    if (await userManager.FindByEmailAsync(defaultEmail) is not null)
    {
        return;
    }

    if (!await roleManager.RoleExistsAsync(defaultRole))
    {
        var role = new Roles { Name = defaultRole };
        var createRoleResult = await roleManager.CreateAsync(role);
        if (!createRoleResult.Succeeded)
        {
            logger.LogWarning(
                "Varsayilan rol '{Role}' olusturulamadi: {Errors}",
                defaultRole,
                string.Join(", ", createRoleResult.Errors.Select(x => x.Description)));

            return;
        }
    }

    var demoUser = User.Create("Demo Kullanici", defaultEmail, company.Id);
    var createResult = await userManager.CreateAsync(demoUser, defaultPassword);
    if (!createResult.Succeeded)
    {
        logger.LogWarning(
            "Demo kullanici olusturulamadi: {Errors}",
            string.Join(", ", createResult.Errors.Select(x => x.Description)));

        return;
    }

    var addRoleResult = await userManager.AddToRoleAsync(demoUser, defaultRole);
    if (!addRoleResult.Succeeded)
    {
        logger.LogWarning(
            "Demo kullaniciya rol atanamadi: {Errors}",
            string.Join(", ", addRoleResult.Errors.Select(x => x.Description)));
    }

    logger.LogInformation("Demo kullanici {Email} olusturuldu.", defaultEmail);

    var defaultPlanId = Guid.Parse("018da123-abcd-7000-9000-000000000001");
    if (!await tenantContext.tenantSubscriptions.AnyAsync(sub => sub.TenantId == company.Id))
    {
        var plan = await tenantContext.companyPlans.FindAsync(defaultPlanId);
        if (plan is null)
        {
            logger.LogWarning("Demo abonelik olusturulamadi: varsayilan plan bulunamadi ({PlanId}).", defaultPlanId);
            return;
        }

        var usage = new TenantUsage(company.Id);
        var subscription = TenantSubscription.CreateActive(
            company.Id,
            defaultPlanId,
            usage.Id,
            "demo-subscription",
            DateTime.UtcNow);

        tenantContext.tenantUsages.Add(usage);
        tenantContext.tenantSubscriptions.Add(subscription);
        await tenantContext.SaveChangesAsync();
        logger.LogInformation("Demo abonelik kaydi olusturuldu.");
    }
}
