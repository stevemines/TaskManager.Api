using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.Api.Data;
using TaskManager.Api.Endpoints;
using TaskManager.Api.Services;

namespace TaskManager.Api
{
    /// <summary>
    /// Entry point for the TaskManager.Api application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configures and starts the web application.
        /// </summary>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the SQLite database context for task storage.
            builder.Services.AddDbContext<TaskDb>(opt => opt.UseSqlite($@"Data Source=Db/TaskList.db"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Register the task manager service for dependency injection.
            builder.Services.AddTransient<ITaskManagerService, TaskManagerService>();

            // Add authorization services.
            builder.Services.AddAuthorization();

            // Add OpenAPI/Swagger services for API documentation.
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager API", Version = "v1" });
            });

            var app = builder.Build();

            // Apply any pending database migrations at startup.
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetService<TaskDb>();
            db?.Database.Migrate();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable OpenAPI and Swagger UI for API documentation.
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }



            // Redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Enable authorization middleware.
            app.UseAuthorization();

            // Map the task manager endpoints under /tasks/v1.
            var plants = app.MapGroup("/tasks/v1")
                .MapTaskManagerEndpointsV1()
                .WithTags("Task Manager Endpoints");

            // Start the web application.
            app.Run();
        }
    }
}
