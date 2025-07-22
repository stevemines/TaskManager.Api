using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    /// <summary>
    /// Service implementation for managing tasks in the database.
    /// </summary>
    public class TaskManagerService : ITaskManagerService
    {
        private readonly TaskDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManagerService"/> class.
        /// </summary>
        /// <param name="db">Database context for tasks.</param>
        public TaskManagerService(TaskDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Checks if a task with the specified title exists (case-insensitive).
        /// </summary>
        /// <param name="title">The title to check for existence.</param>
        /// <returns>True if a task exists with the given title; otherwise, false.</returns>
        public async Task<bool> TaskItemExists(string title)
        {
            return await _db.Tasks.AnyAsync(t => t.Title.ToLower() == title.ToLower());
        }

        /// <summary>
        /// Updates an existing task in the database.
        /// </summary>
        /// <param name="task">The task to update.</param>
        public async Task UpdateTask(TaskItem task)
        {
            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a new task in the database.
        /// </summary>
        /// <param name="task">The task to create.</param>
        public async Task CreateTask(TaskItem task)
        {
            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a task from the database.
        /// </summary>
        /// <param name="task">The task to delete.</param>
        public async Task DeleteTask(TaskItem task)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a task by its unique identifier.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <returns>The matching <see cref="TaskItem"/> or null if not found.</returns>
        public async Task<Models.TaskItem?> GetTask(Guid id) => await _db.Tasks.FindAsync(id);

        /// <summary>
        /// Retrieves all tasks, optionally filtered by status.
        /// </summary>
        /// <param name="status">Optional status to filter tasks.</param>
        /// <returns>A collection of matching tasks.</returns>
        public async Task<IEnumerable<Models.TaskItem>> GetAllTasks(Models.TaskStatus? status = null)
        {
            if (status.HasValue)
            {
                return await _db.Tasks.Where(t => t.Status == status.Value).ToListAsync();
            }

            return await _db.Tasks.ToListAsync();
        }
    }
}
