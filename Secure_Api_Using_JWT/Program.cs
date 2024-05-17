using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Validations;
using Secure_Api_Using_JWT.DbContext;
using Secure_Api_Using_JWT.DbContext.Identity;
using Secure_Api_Using_JWT.Helpers;
using Secure_Api_Using_JWT.Services;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Secure_Api_Using_JWT
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.Configure<JWT>(builder.Configuration.GetSection(nameof(JWT)));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddScoped<IAuthService,AuthService>();
            builder.Services.AddScoped<JWT>();
            builder.Services.AddControllers();
            builder.Services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme =JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme =JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(O =>
            {
                O.RequireHttpsMetadata = false;
                O.SaveToken = false;
                O.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Issuer"],
                    ValidAudience = builder.Configuration["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( builder.Configuration["JWT:KEY"]))
                };

            });
            var app = builder.Build();

            #region Update Database Automatically
            using var scope = app.Services.CreateScope();
            var sevices = scope.ServiceProvider;

            var loggerFactory = sevices.GetService<ILoggerFactory>();
            try
            {
                var dbcontext = sevices.GetRequiredService<ApplicationDbContext>();
                await dbcontext.Database.MigrateAsync();

            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "There's an Error Occure During Apply Database");
            }

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            //var summaries = new[]
            //{
            //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            //};

            //app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            //{
            //    var forecast = Enumerable.Range(1, 5).Select(index =>
            //        new WeatherForecast
            //        {
            //            Date = DateTime.Now.AddDays(index),
            //            TemperatureC = Random.Shared.Next(-20, 55),
            //            Summary = summaries[Random.Shared.Next(summaries.Length)]
            //        })
            //        .ToArray();
            //    return forecast;
            //})
            //.WithName("GetWeatherForecast");
            app.MapControllers();
            app.Run();
        }
    }
}
