
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebApiService.Services;

namespace WebApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthorization(opt =>
            {
                opt.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                opt.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "UsersService",
                        ValidateAudience = true,
                        ValidAudience = "UsersService",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtKey"))
                        ),
                        ValidateIssuerSigningKey = true
                    };
                    opt.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
                    opt.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ev =>
                        {
                            ev.Token = ev.Request.Cookies["jwtToken"];
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddScoped<UsersService>();
            builder.Services.AddDbContext<UsersContext>(opt =>
            {
                var conn = builder.Configuration.GetConnectionString("Pg");
                opt.UseNpgsql(conn);
            });

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
