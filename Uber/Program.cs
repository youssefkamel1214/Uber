
using Authnitication.Database;
using Authnitication.Models.Domain;
using Authnitication.Reposotories;
using Authnitication.Reposotories.interfaces;
using Authnitication.Services;
using Authnitication.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Threading.RateLimiting;
using Uber.Data;
using Uber.Middleware;
using Uber.Models.Domain;
using Uber.Repositories;
using Uber.Repositories.Interfaces;
using Uber.Services;
using Uber.Services.Interfaces;
using Uber.Services.Osm;
using Uber.WebSockets;

namespace Uber
{
    public class Program
    {
        public static void addInjections(WebApplicationBuilder builder) 
        {
            builder.Services.AddDbContext<UberAuthDatabase>(
             options => options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));
        
            builder.Services.AddScoped<IRefershTokenReposotiry<UberAuthDatabase,UberUser>, RefershTokenRepository<UberAuthDatabase, UberUser>>();
            builder.Services.AddScoped<IUserIdentityAuthincation<UberUser>, UserIdentityAuthincation<UberUser>>();
            builder.Services.AddScoped<IAuthniticationService<UberUser,UberAuthDatabase>, AuthniticationService<UberUser, UberAuthDatabase>>();
            builder.Services.AddScoped<IDriverRepository, DriverRepository>(); 
            builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
            builder.Services.AddScoped<ITripRepository, TripRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<ITenderRepository, TenderRepository>();
            builder.Services.AddScoped<ICancellRepository, CanecelltionRepository>();
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IWebSocketService,WebSocketService>();
            builder.Services.AddSingleton<webSocketManager>();
            builder.Services.AddScoped<INotificationManger, NotificationManger>();
            builder.Services.AddHttpClient<IRouteDistanceService, OsmRouteDistanceService>(client =>
            {
                client.BaseAddress = new Uri("http://router.project-osrm.org/");
            });
        }
        public static void addjwtinjectisntions(WebApplicationBuilder builder) {
            var TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudiences = new string[] { builder.Configuration["Jwt:audience"] },
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.
                   SymmetricSecurityKey(System.Text.Encoding.UTF8.
                   GetBytes(builder.Configuration["Jwt:Key"]))
            };
            builder.Services.AddIdentityCore<UberUser>()
               .AddRoles<IdentityRole>()
               .AddTokenProvider<DataProtectorTokenProvider<UberUser>>("Uber")
               .AddEntityFrameworkStores<UberAuthDatabase>()
               .AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = TokenValidationParameters;
           });
            builder.Services.AddSingleton(TokenValidationParameters);
        }
        private static void addMigrations(WebApplication app, Serilog.Core.Logger logger)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<UberAuthDatabase>();
                    dbContext.Database.Migrate();

                    // Creates database and tables
                    // OR for migrations: dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "An error occurred while migrating the database.");
                }
            }
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var logger = new LoggerConfiguration().WriteTo.Console()
            .WriteTo.File("Logs/NzWalklog.txt", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Warning().CreateLogger();
            builder.Logging.AddSerilog(logger);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            addjwtinjectisntions(builder);
            addInjections(builder);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Uber API",
                    Version = "v1",
                });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                    });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>() // Provide an empty list as the required 'value' argument
                }
                });
            });
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("AuthnticationLimiter", httpContext => RateLimitPartition.
                GetFixedWindowLimiter(partitionKey: httpContext.Connection.
                RemoteIpAddress?.ToString(),
                factory: partion => new FixedWindowRateLimiterOptions()
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                }));

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    await context.HttpContext.Response.
                    WriteAsync("Too many requests. Please try again later.",
                    cancellationToken);
                };
            });
            var app = builder.Build();
            addMigrations(app,logger);
           
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseWebSockets();
            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }

        
    }
}
