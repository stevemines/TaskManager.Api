using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;

namespace TaskManager.Tests.Helpers
{
    public class MockDb : IDbContextFactory<TaskDb>
    {
        public TaskDb CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskDb>()
                .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
                .Options;

            return new TaskDb(options);
        }
    }
}
