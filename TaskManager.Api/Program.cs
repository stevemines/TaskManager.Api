
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.Api.Data;
using TaskManager.Api.Endpoints;
using TaskManager.Api.Services;

namespace TaskManager.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add DB Context
            builder.Services.AddDbContext<TaskDb>(opt => opt.UseSqlite($@"Data Source=Db/TaskList.db"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Add application services
            builder.Services.AddTransient<ITaskManagerService, TaskManagerService>();

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager API", Version = "v1" });
            });

            var app = builder.Build();

            //Add or update DB
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetService<TaskDb>();
            db?.Database.Migrate();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var plants = app.MapGroup("/tasks/v1")
           .MapTaskManagerEndpointsV1()
           .WithTags("Task Manager Endpoints");

            app.Run();
        }
    }
}
