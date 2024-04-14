using System.Data.SqlClient;
using EchiBackendServices.Services;
using Dapper;
using EchiBackendServices.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;

namespace EchiBackendServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var sqlConnString = Environment.GetEnvironmentVariable("SqlConnString");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSingleton<DocumentService>();
            builder.Services.AddSingleton<AzureBlobStorageService>();
            builder.Services.AddSingleton(new ConnectionStringStore()
            {
                SqlConnectionString = sqlConnString
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(sqlConnString);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
