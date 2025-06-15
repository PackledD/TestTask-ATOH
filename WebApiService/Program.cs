
using DataAccess;
using Microsoft.EntityFrameworkCore;
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

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
