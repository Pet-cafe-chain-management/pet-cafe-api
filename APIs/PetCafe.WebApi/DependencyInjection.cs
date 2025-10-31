using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCafe.Application;
using PetCafe.Application.GlobalExceptionHandling;
using PetCafe.Application.Mappers;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Infrastructures;
using PetCafe.WebApi.Middlewares;
using PetCafe.WebApi.Services;
using Scrutor;
namespace PetCafe.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiService(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddScoped<IClaimsService, ClaimService>();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
                    appSettings.ConnectionStrings.PostgreConnection ?? throw new InvalidOperationException("Connection string 'PostgreConnection' not found.")));
        
        // Register MongoDB context
        services.AddScoped<Infrastructures.MongoDbContext>();
        
        services.AddAutoMapper(typeof(MapperConfigurationsProfile));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers();
        services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new OrderParamListBinderProvider());
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
            options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

        });

        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy
                => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            }
        );

        services.Scan(scan =>
        {
            scan.FromAssemblies(GetAssemblies())
                .AddClasses()
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithScopedLifetime();
        });


        services.AddValidatorsFromAssemblies(assemblies: GetAssemblies());
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings.JWTOptions.Issuer,
                    ValidAudience = appSettings.JWTOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTOptions.SecretKey)),
                    ClockSkew = TimeSpan.FromSeconds(1)
                };
            });

        services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

        services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                        .ToList();

                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    var exceptionResult = JsonSerializer.Serialize(new
                    {
                        error = errors
                    }, jsonOptions);

                    return new BadRequestObjectResult(exceptionResult);
                };
            });

        services.AddSingleton<GlobalErrorHandlingMiddleware>();
        services.AddSingleton<PerformanceMiddleware>();
        services.AddSingleton<Stopwatch>();

        services.AddSignalR();
        services.AddHangfire(config => config.UseInMemoryStorage());
        services.AddHangfireServer();

        return services;
    }

    private static Assembly[] GetAssemblies()
        => [Infrastructures.AssemblyReference.Assembly, Application.AssemblyReference.Assembly];

}