using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Endpoints
{
    /// <summary>
    /// Provides version 1 endpoints for task management operations.
    /// </summary>
    public static class TaskManagerEndpointsV1
    {
        /// <summary>
        /// Maps all task management endpoints to the provided route group.
        /// </summary>
        public static RouteGroupBuilder MapTaskManagerEndpointsV1(this RouteGroupBuilder group)
        {
            group.MapDelete("/{id}", DeleteTask); // Delete a task by ID
            group.MapPut("/{id}", UpdateTask); // Update a task by ID
            group.MapPut("/{id}/Complete", CompleteTask); // Mark a task as completed
            group.MapPost("/", CreateTask); // Create a new task
            group.MapGet("/{id}", GetTask); // Get a task by ID
            group.MapGet("/", GetAllTasks); // Get all tasks
            group.MapGet("/Completed", GetAllCompletedTasks); // Get all completed tasks
            group.MapGet("/Pending", GetAllPendingTasks); // Get all pending tasks

            return group;
        }

        /// <summary>
        /// Marks the specified task as completed.
        /// </summary>
        public static async Task<Results<Ok<TaskItem>, BadRequest<string>, NotFound>> CompleteTask(Guid id, ITaskManagerService taskManagerService)
        {
            try
            {
                if (await taskManagerService.GetTask(id) is TaskItem task)
                {
                    task.Status = Models.TaskStatus.Completed;
                    task.UpdatedAt = DateTime.UtcNow;

                    await taskManagerService.UpdateTask(task);

                    return TypedResults.Ok(task);
                }

                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes the specified task.
        /// </summary>
        public static async Task<Results<NoContent, BadRequest<string>, NotFound>> DeleteTask(Guid id, ITaskManagerService taskManagerService)
        {
            try
            {
                if (await taskManagerService.GetTask(id) is TaskItem task)
                {
                    await taskManagerService.DeleteTask(task);
                    return TypedResults.NoContent();
                }

                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates the specified task with new data.
        /// </summary>
        public static async Task<Results<Ok<TaskItem>, Conflict<string>, BadRequest<string>, NotFound>> UpdateTask(Guid id, TaskCreateUpdateDto taskUpdate, ITaskManagerService taskManagerService)
        {
            try
            {
                if (await taskManagerService.GetTask(id) is TaskItem task)
                {
                    // Prevent duplicate titles
                    if (!task.Title.Equals(taskUpdate.Title, StringComparison.OrdinalIgnoreCase) && await taskManagerService.TaskItemExists(taskUpdate.Title))
                    {
                        return TypedResults.Conflict("Task with the same title already exists.");
                    }

                    task.Title = taskUpdate.Title;
                    task.Description = taskUpdate.Description;
                    task.UpdatedAt = DateTime.UtcNow;

                    await taskManagerService.UpdateTask(task);

                    return TypedResults.Ok(task);
                }

                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        public static async Task<Results<Created<TaskItem>, Conflict<string>, BadRequest<string>>> CreateTask(TaskCreateUpdateDto taskCreate, ITaskManagerService taskManagerService)
        {
            try
            {
                // Prevent duplicate titles
                if (await taskManagerService.TaskItemExists(taskCreate.Title))
                {
                    return TypedResults.Conflict("Task with the same title already exists.");
                }

                var task = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = taskCreate.Title,
                    Description = taskCreate.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Status = Models.TaskStatus.Pending
                };
                await taskManagerService.CreateTask(task);

                return TypedResults.Created($"/tasks/v1/{task.Id}", task);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a task by its ID.
        /// </summary>
        public static async Task<Results<Ok<TaskItem>, BadRequest<string>, NotFound>> GetTask(Guid id, ITaskManagerService taskManagerService)
        {
            try
            {
                var task = await taskManagerService.GetTask(id);
                if (task != null)
                {
                    return TypedResults.Ok(task);
                }
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all tasks.
        /// </summary>
        public static async Task<Results<Ok<List<TaskItem>>, BadRequest<string>, NotFound>> GetAllTasks(ITaskManagerService taskManagerService)
        {
            try
            {
                var tasks = await taskManagerService.GetAllTasks();
                if (tasks.Any())
                {
                    return TypedResults.Ok(tasks.ToList());
                }
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all pending tasks.
        /// </summary>
        public static async Task<Results<Ok<List<TaskItem>>, BadRequest<string>, NotFound>> GetAllPendingTasks(ITaskManagerService taskManagerService)
        {
            try
            {
                var tasks = await taskManagerService.GetAllTasks(Models.TaskStatus.Pending);
                if (tasks.Any())
                {
                    return TypedResults.Ok(tasks.ToList());
                }
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all completed tasks.
        /// </summary>
        public static async Task<Results<Ok<List<TaskItem>>, BadRequest<string>, NotFound>> GetAllCompletedTasks(ITaskManagerService taskManagerService)
        {
            try
            {
                var tasks = await taskManagerService.GetAllTasks(Models.TaskStatus.Completed);
                if (tasks.Any())
                {
                    return TypedResults.Ok(tasks.ToList());
                }
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
    }
}
