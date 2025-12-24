using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Runtime.CompilerServices;
using System.Text;
using TimeOfEnter.DateTimeMiddlleWare;
using TimeOfEnter.Helper;
using TimeOfEnter.Model;
using TimeOfEnter.Repository;
using TimeOfEnter.Service;

namespace TimeOfEnter.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<TestContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("cs"));
        });

        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<TestContext>()
            .AddDefaultTokenProviders();

        var jwt = configuration.GetSection("JWT").Get<JWT>();

        services.AddAuthentication(Options =>
        {
            Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            Options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(Options =>
        {
            Options.SaveToken = true;
            Options.RequireHttpsMetadata = false;
            Options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.IssuerIP,
                ValidateAudience = true,
                ValidAudience = jwt.AudienceIP,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt.SecritKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());
            });

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(
                configuration.GetConnectionString("cs"));
        });

        services.AddHangfireServer();

        services.Configure<JWT>(configuration.GetSection("JWT"));

        services.AddFluentValidation();

        services.AddScoped<IDateRepository, DateRepository>();
        services.AddScoped<IDateService, DateService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICleanNoneActiveDateService, CleanNoneActiveDateService>();
        services.AddScoped<UpdateActivationOfDateService>();

        services.AddSwaggerGen(swagger =>
        {
            //This is to generate the Default UI of Swagger Documentation
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ASP.NET 8 Web API",
                Description = " ITI Projrcy"
            });
            // To Enable authorization using Swagger (JWT)
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] {}
                }
                });
        });

        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}

