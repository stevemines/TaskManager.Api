using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public class TaskManagerService: ITaskManagerService
    {
        private readonly TaskDb _db;
        public TaskManagerService(TaskDb db)
        {
            _db = db;
        }

        public async Task<bool> TaskItemExists(string title)
        {
            return await _db.Tasks.AnyAsync(t => t.Title.ToLower() == title.ToLower());
        }

        public async Task UpdateTask(TaskItem task)
        {
            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
        }
        public async Task CreateTask(TaskItem task)
        {
            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();

        }
        public async Task DeleteTask(TaskItem task)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }
        public async Task<Models.TaskItem?> GetTask(Guid id) => await _db.Tasks.FindAsync(id);
        public async Task<IEnumerable<Models.TaskItem>> GetAllTasks(Models.TaskStatus? status = null)
        {
            if(status.HasValue)
            {
                return await _db.Tasks.Where(t => t.Status == status.Value).ToListAsync();
            }

            return await _db.Tasks.ToListAsync();
        }
    }
}
