using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public sealed class TaskServiceTests : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _dbContext;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();
        _service = new TaskService(_dbContext);
    }

    [Fact]
    public async Task CreateAndGetTasks_AreScopedToUser()
    {
        await AddUserAsync("user-a");
        await AddUserAsync("user-b");
        await _service.CreateTaskAsync(new TaskItem { Title = "  First task  " }, "user-a");
        await _service.CreateTaskAsync(new TaskItem { Title = "Other task" }, "user-b");

        var tasks = await _service.GetTasksAsync("user-a");

        var task = Assert.Single(tasks);
        Assert.Equal("First task", task.Title);
        Assert.Equal("user-a", task.UserId);
    }

    [Fact]
    public async Task GetTask_DoesNotExposeAnotherUsersTask()
    {
        await AddUserAsync("owner");
        await AddUserAsync("attacker");
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Private" }, "owner");

        Assert.Null(await _service.GetTaskAsync(created.Id, "attacker"));
    }

    [Fact]
    public async Task Update_DoesNotModifyAnotherUsersTask()
    {
        await AddUserAsync("owner");
        await AddUserAsync("attacker");
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Original" }, "owner");

        var updated = await _service.UpdateTaskAsync(
            new TaskItem { Id = created.Id, Title = "Changed" }, "attacker");

        Assert.False(updated);
        Assert.Equal("Original", (await _service.GetTaskAsync(created.Id, "owner"))!.Title);
    }

    [Fact]
    public async Task CompletionAndDelete_DeniedForAnotherUser()
    {
        await AddUserAsync("owner");
        await AddUserAsync("attacker");
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Private" }, "owner");

        Assert.False(await _service.SetCompletionAsync(created.Id, true, "attacker"));
        Assert.False(await _service.DeleteTaskAsync(created.Id, "attacker"));
        Assert.NotNull(await _service.GetTaskAsync(created.Id, "owner"));
    }

    [Fact]
    public async Task UpdateCompletionAndDelete_PersistForOwner()
    {
        await AddUserAsync("owner");
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Initial" }, "owner");

        Assert.True(await _service.UpdateTaskAsync(
            new TaskItem { Id = created.Id, Title = "Updated", Description = " Details " }, "owner"));
        Assert.True(await _service.SetCompletionAsync(created.Id, true, "owner"));
        var updated = await _service.GetTaskAsync(created.Id, "owner");
        Assert.Equal("Updated", updated!.Title);
        Assert.Equal("Details", updated.Description);
        Assert.True(updated.IsCompleted);

        Assert.True(await _service.DeleteTaskAsync(created.Id, "owner"));
        Assert.Null(await _service.GetTaskAsync(created.Id, "owner"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_RejectsBlankTitles(string title)
    {
        await AddUserAsync("owner");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateTaskAsync(new TaskItem { Title = title }, "owner"));
    }

    private async Task AddUserAsync(string id)
    {
        _dbContext.Users.Add(new ApplicationUser
        {
            Id = id,
            UserName = $"{id}@example.com",
            NormalizedUserName = $"{id}@example.com".ToUpperInvariant(),
            Email = $"{id}@example.com",
            NormalizedEmail = $"{id}@example.com".ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString()
        });
        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
