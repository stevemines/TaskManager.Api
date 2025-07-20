using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api;
using TaskManager.Api.Endpoints;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Tests
{
    public class TaskMoqTests
    {

        [Fact]
        public async Task GetTaskReturnsNotFoundIfNotExists()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid testId = Guid.NewGuid();

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await TaskManagerEndpointsV1.GetTask(testId, mock.Object);

            //Assert
            Assert.IsType<Results<Ok<TaskItem>, BadRequest<string>, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;

            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task DeleteTaskReturnsNotFoundIfNotExists()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid testId = Guid.NewGuid();

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await TaskManagerEndpointsV1.DeleteTask(testId, mock.Object);

            //Assert
            Assert.IsType<Results<NoContent, BadRequest<string>, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;

            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task UpdateTaskReturnsNotFoundIfNotExists()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid testId = Guid.NewGuid();

            var updateTask = new TaskCreateUpdateDto
            { Description = "Test description", Title = "Test title" };

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await TaskManagerEndpointsV1.UpdateTask(testId, updateTask, mock.Object);

            //Assert
            Assert.IsType<Results<Ok<TaskItem>, Conflict<string>, BadRequest<string>, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;

            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task UpdateTaskReturnsConflictIfExists()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid testId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = testId,
                Title = "Exiting test title",
                UpdatedAt = null,
                Status = Api.Models.TaskStatus.Pending
            };

            var updateTask = new TaskCreateUpdateDto
            {
                Description = "Test description",
                Title = "Test title"
            };

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                            .ReturnsAsync(existingTask);

            mock.Setup(m => m.TaskItemExists(It.Is<string>(title => title == updateTask.Title)))
                .ReturnsAsync(true);

            // Act
            var result = await TaskManagerEndpointsV1.UpdateTask(testId, updateTask, mock.Object);

            //Assert
            Assert.IsType<Results<Ok<TaskItem>, Conflict<string>, BadRequest<string>, NotFound>>(result);

            var conflictResult = (Conflict<string>)result.Result;

            Assert.Equal("Task with the same title already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task CreateTaskReturnsConflictIfExists()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            var createTask = new TaskCreateUpdateDto
            { Description = "Test description", Title = "Test title" };

            mock.Setup(m => m.TaskItemExists(It.Is<string>(title => title == createTask.Title)))
                .ReturnsAsync(true);

            // Act
            var result = await TaskManagerEndpointsV1.CreateTask(createTask, mock.Object);

            //Assert
            Assert.IsType<Results<Created<TaskItem>, Conflict<string>, BadRequest<string>>>(result);

            var conflictResult = (Conflict<string>)result.Result;

            Assert.Equal("Task with the same title already exists.", conflictResult.Value);
        }


        [Fact]
        public async Task GetAllReturnsTasksFromDatabase()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid test1Id = Guid.NewGuid();
            Guid test2Id = Guid.NewGuid();

            mock.Setup(m => m.GetAllTasks(It.Is<Api.Models.TaskStatus?>(s=> s==null)))
                .ReturnsAsync(new List<TaskItem> {
                new TaskItem
                {
                    Id = test1Id,
                    Title = "Test Task 1",
                    Description = "This is a test task 1.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Status = Api.Models.TaskStatus.Pending
                },
                new TaskItem
                {
                    Id = test2Id,
                    Title = "Test Task 2",
                    Description = "This is a test task 2.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Status = Api.Models.TaskStatus.Completed
                } });

            // Act
            var result = await TaskManagerEndpointsV1.GetAllTasks(mock.Object);

            //Assert
            Assert.IsType<Results<Ok<List<TaskItem>>, BadRequest<string>, NotFound>>(result);

            var okResult = (Ok<List<TaskItem>>)result.Result;

            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.NotEmpty(okResult.Value);
            Assert.Equal(2, okResult.Value.Count);
            Assert.Collection(okResult.Value, task1 =>
            {
                Assert.Equal(test1Id, task1.Id);
                Assert.Equal("Test Task 1", task1.Title);
                Assert.Equal(Api.Models.TaskStatus.Pending, task1.Status);
            }, task2 =>
            {
                Assert.Equal(test2Id, task2.Id);
                Assert.Equal("Test Task 2", task2.Title);
                Assert.Equal(Api.Models.TaskStatus.Completed, task2.Status);
            });
        }

        [Fact]
        public async Task GetAllPendingReturnsTasksFromDatabase()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid test1Id = Guid.NewGuid();

            mock.Setup(m => m.GetAllTasks(It.Is<Api.Models.TaskStatus?>(s => s == Api.Models.TaskStatus.Pending)))
                .ReturnsAsync(new List<TaskItem> {
                new TaskItem
                {
                    Id = test1Id,
                    Title = "Test Task 1",
                    Description = "This is a test task 1.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Status = Api.Models.TaskStatus.Pending
                }});

            // Act
            var result = await TaskManagerEndpointsV1.GetAllPendingTasks(mock.Object);

            //Assert
            Assert.IsType<Results<Ok<List<TaskItem>>, BadRequest<string>, NotFound>>(result);

            var okResult = (Ok<List<TaskItem>>)result.Result;

            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.NotEmpty(okResult.Value);
            Assert.Equal(1, okResult.Value.Count);
            Assert.Collection(okResult.Value, task1 =>
            {
                Assert.Equal(test1Id, task1.Id);
                Assert.Equal("Test Task 1", task1.Title);
                Assert.Equal(Api.Models.TaskStatus.Pending, task1.Status);
            });
        }

        [Fact]
        public async Task GetAllCompletedReturnsTasksFromDatabase()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid test1Id = Guid.NewGuid();

            mock.Setup(m => m.GetAllTasks(It.Is<Api.Models.TaskStatus?>(s => s == Api.Models.TaskStatus.Completed)))
                .ReturnsAsync(new List<TaskItem> {
                new TaskItem
                {
                    Id = test1Id,
                    Title = "Test Task 1",
                    Description = "This is a test task 1.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Status = Api.Models.TaskStatus.Pending
                }});

            // Act
            var result = await TaskManagerEndpointsV1.GetAllCompletedTasks(mock.Object);

            //Assert
            Assert.IsType<Results<Ok<List<TaskItem>>, BadRequest<string>, NotFound>>(result);

            var okResult = (Ok<List<TaskItem>>)result.Result;

            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            Assert.NotEmpty(okResult.Value);
            Assert.Equal(1, okResult.Value.Count);
            Assert.Collection(okResult.Value, task1 =>
            {
                Assert.Equal(test1Id, task1.Id);
                Assert.Equal("Test Task 1", task1.Title);
                Assert.Equal(Api.Models.TaskStatus.Completed, task1.Status);
            });
        }


        [Fact]
        public async Task GetTodoReturnsTaskItemIfExists()
        {
            // Arrange
            var mock = new Mock<ITaskManagerService>();

            Guid testId = Guid.NewGuid();

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                .ReturnsAsync(new TaskItem
                {
                    Id = testId,
                    Title = "Test Task",
                    Description = "This is a test task.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    Status = Api.Models.TaskStatus.Pending
                });

            // Act
            var result = await TaskManagerEndpointsV1.GetTask(testId, mock.Object);

            //Assert
            Assert.IsType<Results<Ok<TaskItem>, BadRequest<string>, NotFound>>(result);

            var okResult = (Ok<TaskItem>)result.Result;

            Assert.NotNull(okResult);

            Assert.Equal(testId, okResult.Value.Id);
        }

        [Fact]
        public async Task CreateTaskCreatesTaskInDatabase()
        {
            //Arrange
            var tasks = new List<TaskItem>();

            var newTask = new TaskCreateUpdateDto
            {
                Title = "Test title",
                Description = "Test description"
            };

            var mock = new Mock<ITaskManagerService>();

            mock.Setup(m => m.CreateTask(It.Is<TaskItem>(t => t.Title == newTask.Title && t.Description == newTask.Description)))
                .Callback<TaskItem>(task => tasks.Add(task))
                .Returns(Task.FromResult<Guid>(Guid.NewGuid()));

            //Act
            var result = await TaskManagerEndpointsV1.CreateTask(newTask, mock.Object);

            //Assert
            Assert.IsType<Results<Created<TaskItem>, Conflict<string>, BadRequest<string>>>(result);

            Assert.NotNull(result);

            Assert.NotEmpty(tasks);
            Assert.Collection(tasks, task =>
            {
                Assert.Equal("Test title", task.Title);
                Assert.Equal("Test description", task.Description);
                Assert.Equal(Api.Models.TaskStatus.Pending, task.Status);
            });
        }

        [Fact]
        public async Task UpdateTaskUpdatesTaskInDatabase()
        {
            //Arrange
            Guid testId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = testId,
                Title = "Exiting test title",
                UpdatedAt = null,
                Status = Api.Models.TaskStatus.Pending
            };

            var updatedTask = new TaskItem
            {
                Id = testId,
                Title = "Updated test title",
                Description = "Updated test description",
                Status = Api.Models.TaskStatus.Pending
            };

            var updatedTaskDto = new TaskCreateUpdateDto
            {
                Title = "Updated test title",
                Description = "Updated test description"

            };

            var mock = new Mock<ITaskManagerService>();

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                .ReturnsAsync(existingTask);

            mock.Setup(m => m.UpdateTask(It.Is<TaskItem>(t => t.Id == updatedTask.Id && t.Description == updatedTask.Description && t.Title == updatedTask.Title)))
                .Callback<TaskItem>(task => existingTask = task)
                .Returns(Task.CompletedTask);

            //Act
            var result = await TaskManagerEndpointsV1.UpdateTask(testId, updatedTaskDto, mock.Object);

            //Assert
            Assert.IsType<Results<Ok<TaskItem>, Conflict<string>, BadRequest<string>, NotFound>>(result);

            var createdResult = (Ok<TaskItem>)result.Result;

            Assert.NotNull(createdResult);

            Assert.Equal("Updated test title", createdResult.Value.Title);
            Assert.Equal(Api.Models.TaskStatus.Pending, createdResult.Value.Status);
            Assert.NotNull(createdResult.Value.UpdatedAt);
        }

        [Fact]
        public async Task CompleteTaskUpdatesTaskInDatabase()
        {
            //Arrange
            Guid testId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = testId,
                Title = "Exiting test title",
                UpdatedAt = null,
                Status = Api.Models.TaskStatus.Pending
            };


            var mock = new Mock<ITaskManagerService>();

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == testId)))
                .ReturnsAsync(existingTask);

            mock.Setup(m => m.UpdateTask(It.Is<TaskItem>(t => t.Id == existingTask.Id && t.Description == existingTask.Description && t.Title == existingTask.Title)))
                .Callback<TaskItem>(task => existingTask = task)
                .Returns(Task.CompletedTask);

            //Act
            var result = await TaskManagerEndpointsV1.CompleteTask(testId, mock.Object);

            //Assert
            Assert.IsType<Results<Ok<TaskItem>, BadRequest<string>, NotFound>>(result);

            var createdResult = (Ok<TaskItem>)result.Result;

            Assert.NotNull(createdResult);

            Assert.Equal(Api.Models.TaskStatus.Completed, existingTask.Status);
            Assert.NotNull(existingTask.UpdatedAt);
        }

        [Fact]
        public async Task DeleteTaskDeletesTaskInDatabase()
        {
            //Arrange
            Guid testId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = testId,
                Title = "Test title",
                Description = "Test description",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                Status = Api.Models.TaskStatus.Pending
            };


            var Tasks = new List<TaskItem> { existingTask };

            var mock = new Mock<ITaskManagerService>();

            mock.Setup(m => m.GetTask(It.Is<Guid>(id => id == existingTask.Id)))
                .ReturnsAsync(existingTask);

            mock.Setup(m => m.DeleteTask(It.Is<TaskItem>(t => t.Id == testId)))
                .Callback<TaskItem>(task => Tasks.Remove(task))
                .Returns(Task.CompletedTask);

            //Act
            var result = await TaskManagerEndpointsV1.DeleteTask(existingTask.Id, mock.Object);

            //Assert
            Assert.IsType<Results<NoContent, BadRequest<string>, NotFound>>(result);

            var noContentResult = (NoContent)result.Result;

            Assert.NotNull(noContentResult);
            Assert.Empty(Tasks);
        }

    }
}
