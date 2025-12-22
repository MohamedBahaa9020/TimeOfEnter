using TimeOfEnter.Model;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using TimeOfEnter.DateTimeMiddlleWare;
using TimeOfEnter.Helper;
using TimeOfEnter.Middleware;
using TimeOfEnter.Model;
using TimeOfEnter.Repository;
using TimeOfEnter.Service;
using TimeOfEnter.Validation;

namespace TimeOfEnter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<TestContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<TestContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(Options =>
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
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecritKey"])),
                    ClockSkew=TimeSpan.Zero
                };
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new NullableDateTimeConverter());

                });
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<DateValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<AddUserRole>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginValid>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterValid>();

            builder.Services.AddScoped<IDateRepository, DateRepository>();

            builder.Services.AddScoped<IDateService, DateService>();
            builder.Services.AddScoped<IAccountService, AccountService>();


            builder.Services.AddSwaggerGen(swagger =>
            {
                //This�is�to�generate�the�Default�UI�of�Swagger�Documentation����
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET�8�Web�API",
                    Description = " ITI Projrcy"
                });
                //�To�Enable�authorization�using�Swagger�(JWT)����
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter�'Bearer'�[space]�and�then�your�valid�token�in�the�text�input�below.\r\n\r\nExample:�\"Bearer�eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
