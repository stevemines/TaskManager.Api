using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public interface ITaskManagerService
    {
        Task<bool> TaskItemExists(string title);
        Task UpdateTask(TaskItem task);
        Task CreateTask(TaskItem task);
        Task DeleteTask(TaskItem task);
        Task<TaskItem?> GetTask(Guid id);
        Task<IEnumerable<TaskItem>> GetAllTasks(Models.TaskStatus? status = null);

    }
}
