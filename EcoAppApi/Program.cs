
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

namespace EcoAppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            //var secretKey = jwtSettings["SecretKey"];
            //if (string.IsNullOrEmpty(secretKey))
            //{
            //    byte[] key = new byte[64]; // 512 bits = 64 bytes
            //    using (var rng = RandomNumberGenerator.Create())
            //    {
            //        rng.GetBytes(key); // Fill the array with cryptographically secure random bytes
            //    }

            //    secretKey = Convert.ToBase64String(key);

            //    builder.Configuration["JwtSettings:SecretKey"] = secretKey;
            //    Console.WriteLine("Generated Key (Base64):");
            //    Console.WriteLine(secretKey);
            //}

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings["SecretKey"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
   
                };
                options.MapInboundClaims = false;
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
            //builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<PricingService>();
            builder.Services.AddScoped<JwtUtils>();
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddScoped<IUserContextService, UserContextService>();


            builder.Services.AddLogging();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
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
                }
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
            app.UseCors(corsPolicy);

            app.UseAuthentication();

            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();
            var keyBytes = Convert.FromBase64String("QUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUE=");
            Console.WriteLine($"Key Length: {keyBytes.Length} bytes");

            app.Run();
        }
    }
}
