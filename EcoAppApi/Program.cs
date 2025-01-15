
using EcoAppApi.Utils;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcoAppApi.Utils;
using EcoAppApi.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

namespace EcoAppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();


            // Add services to the container.
            builder.Services.AddDbContext<DALContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DALContext") ?? throw new InvalidOperationException("Connection string 'DALContext' not found.")));
            builder.Services.AddIdentity<AppUser, IdentityRole<string>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;

            }).AddEntityFrameworkStores<DALContext>();
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
             });
            //JWT Setup:
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
          

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                   
  
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("Hj7xEhjlgMNrak17t5Q2HIvp+YN9wVtar+UpxyNMssua9rD/a7rIpabVnmQKvm1Ea6WJBR8GRTuDin3F7tlPxg=="))

                };
             
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        string sanitizedMessage = context.Exception.Message.Replace("\r", "").Replace("\n", "").Trim();
                        context.NoResult();
                        context.Response.Headers.Add("Token-Error", sanitizedMessage);
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddScoped<ProductsRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<PricingService>();
            builder.Services.AddScoped<JwtUtils>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<DALContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DALContext")));
            //builder.Services.AddScoped<IUserContextService, UserContextService>();


            builder.Services.AddLogging();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                  {
                   {
                          new OpenApiSecurityScheme
                          {
                           Reference = new OpenApiReference
                            {
                             Type = ReferenceType.SecurityScheme,
                              Id = "Bearer"
                             },
                             Scheme = "Bearer",
                             Name = "Bearer",
                             In = ParameterLocation.Header
                            },
                             new string[] {}
                   }
                  });
            });
            builder.Logging.AddConsole();

            var corsPolicy = "CorsPolicy";

            builder.Services.AddCors(option =>
            {
                option.AddPolicy(name: corsPolicy, policy =>
                {
                    policy.WithOrigins(
                       "https://localhost:7129",
                        "http://localhost:3000",
                        "http://localhost:5173",
                        "http://localhost:5174"
                    ).AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            var app = builder.Build();
            var logger = app.Logger;


            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                logger.LogInformation($"Request Body: {body}");
                context.Request.Body.Position = 0; // Reset stream for the next middleware
                await next();
            });
            app.Use(async (context, next) =>
            {
                logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await next();
                logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
            });
            app.Use(async (context, next) =>
            {
                Console.WriteLine("Request Headers:");
                foreach (var header in context.Request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }

                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    Console.WriteLine("Authorization header found");
                }

                await next();
            });
            app.UseCors(corsPolicy);
            app.Use(async (context, next) =>
            {
                var userClaims = context.User.Claims;
                foreach (var claim in userClaims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
                await next();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();
       

            app.Run();
        }
    }
}
