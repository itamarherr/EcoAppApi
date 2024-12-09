
using ApiExercise.Services;
using DAL.Data;
using DAL.Models;
using EcoAppApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcoAppApi.Utils;

namespace ApiExercise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<DALContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DALContext") ?? throw new InvalidOperationException("Connection string 'DALContext' not found.")));
            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;

            }).AddEntityFrameworkStores<DALContext>();

            //JWT Setup:
            var jwtSettings = builder.Configuration.GetSection("JWTSettings");
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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
                };
            });

            builder.Services.AddScoped<ProductsRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<PricingService>();


            builder.Services.AddScoped<JwtUtils>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var corsPolicy = "CorsPolicy";

            builder.Services.AddCors(option =>
            {
                option.AddPolicy(name: corsPolicy, policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000",
                        "http://localhost:5173",
                        "http://localhost:5174"
                    ).AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseCors(corsPolicy);

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
