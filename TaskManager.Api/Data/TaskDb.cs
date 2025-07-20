using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data
{
    public class TaskDb : DbContext
    {
        public TaskDb(DbContextOptions<TaskDb> options)
            : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}