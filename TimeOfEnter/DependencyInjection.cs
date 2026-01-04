using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TimeOfEnter.Database;
using TimeOfEnter.Infrastructure.Convertors;
using TimeOfEnter.Infrastructure.Options;
using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;
namespace TimeOfEnter;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<DateContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("cs"));
        });

        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<DateContext>()
            .AddDefaultTokenProviders();

        var jwt = configuration.GetSection("JWT").Get<JwtOptions>();

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
                ValidIssuer = jwt!.IssuerIP,
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

        services.Configure<JwtOptions>(configuration.GetSection("JWT"));

        services.AddFluentValidation();

        services.AddScoped<IDateRepository, DateRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IDateService, DateService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ASP.NET 8 Web API",
                Description = " ITI Projrcy"
            });
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
                Array.Empty<string>()
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

