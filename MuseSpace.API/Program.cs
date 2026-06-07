using System.Text;
using System.Threading.RateLimiting;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MuseSpace.API.Middleware;
using MuseSpace.API.Services;
using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Helper;
using MuseSpace.BLL.Mappings;
using MuseSpace.BLL.Response;
using MuseSpace.BLL.Services;
using MuseSpace.BLL.Utilities;
using MuseSpace.Core.Interfaces.Helper;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.BLL.Interfaces.Services;
using MuseSpace.Core.Interfaces.Services;
using MuseSpace.Infrastructure.Data;
using MuseSpace.Infrastructure.Repositories;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/musespace-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Connection string 'Postgres' not found");
builder.Services.AddDbContext<MuseSpaceDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(3);
        npgsqlOptions.CommandTimeout(30);
    }));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = builder.Configuration.GetValue<string>("Redis:InstanceName") ?? "MuseSpace_";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MuseSpace API",
        Version = "v1",
        Description = "API for MuseSpace - A platform for art.",
        Contact = new OpenApiContact
        {
            Name = "MuseSpace",
            Email = "noreply@example.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "MuseSpace.API.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }

    options.EnableAnnotations();
});

builder.Services.AddScoped<IService<SampleDto>, SampleService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IPasswordHashHelper, BCryptPasswordHashHelper>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IArtworkRepository, ArtworkRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthResponseFactory, AuthResponseFactory>();
builder.Services.AddScoped<IOtpService>(sp =>
    new OtpService(
        sp.GetRequiredService<IOtpRepository>(),
        sp.GetRequiredService<IDateTimeProvider>(),
        builder.Configuration));
builder.Services.AddScoped<IMediaUploadService, MuseSpace.Infrastructure.ExternalServices.CloudinaryMediaUploadService>();
builder.Services.AddScoped<IArtworkService, ArtworkService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ISocialService, SocialService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationDispatcher, SignalRNotificationDispatcher>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPaymentService, QrisPaymentService>();
builder.Services.AddHttpClient<IAiDescriptionService, GeminiDescriptionService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
    cfg.AddProfile<AuthMappingProfile>();
    cfg.AddProfile<ArtworkMappingProfile>();
    cfg.AddProfile<GroupMappingProfile>();
    cfg.AddProfile<EventMappingProfile>();
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MuseSpace API v1");
        options.RoutePrefix = "swagger";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DisplayOperationId();
    });
}

app.UseHttpsRedirection();
app.UseRequestLogging();

app.UseRateLimiter();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MuseSpace.API.Hubs.NotificationHub>("/hubs/notifications");

using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MuseSpaceDbContext>();
    try
    {
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied successfully");

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await dbContext.SeedRolesAsync();
                await dbContext.SeedTagsAsync();
                await dbContext.SaveChangesAsync();

                await dbContext.SeedUsersAsync();
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Database seeding failed: {ex.Message}");
                throw;
            }
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
        throw;
    }
}

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
