using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using blazor_todo_list.Components;

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
        _service = new TaskService(_dbContext, NullLogger<TaskService>.Instance);
    }

    [Fact]
    public async Task CreateAndGetTasks_AreScopedToUser()
    {
        await AddUserAsync("user-a");
        await AddUserAsync("user-b");
        await _service.AddAsync("user-a", new TaskFormValue { Title = "  First task  " });
        await _service.AddAsync("user-b", new TaskFormValue { Title = "Other task" });

        var tasks = await _service.GetAllForUserAsync("user-a");

        var task = Assert.Single(tasks);
        Assert.Equal("First task", task.Title);
    }

    [Fact]
    public async Task GetTask_DoesNotExposeAnotherUsersTask()
    {
        await AddUserAsync("owner");
        await AddUserAsync("attacker");
        var created = await _service.AddAsync("owner", new TaskFormValue { Title = "Private" });

        Assert.Null(await _service.GetByIdAsync("attacker", created.Id));
    }

    [Fact]
    public async Task Update_DoesNotModifyAnotherUsersTask()
    {
        await AddUserAsync("owner");
        await AddUserAsync("attacker");
        var created = await _service.AddAsync("owner", new TaskFormValue { Title = "Original" });

        var updated = await _service.UpdateAsync(
            "attacker", created.Id, new TaskFormValue { Title = "Changed" });

        Assert.Null(updated);
        Assert.Equal("Original", (await _service.GetByIdAsync("owner", created.Id))!.Title);
    }

    [Fact]
    public async Task CompletionAndDelete_DeniedForAnotherUser()
    {
        await AddUserAsync("owner");
        await AddUserAsync("attacker");
        var created = await _service.AddAsync("owner", new TaskFormValue { Title = "Private" });

        Assert.False(await _service.ToggleStatusAsync("attacker", created.Id));
        Assert.False(await _service.DeleteAsync("attacker", created.Id));
        Assert.NotNull(await _service.GetByIdAsync("owner", created.Id));
    }

    [Fact]
    public async Task UpdateCompletionAndDelete_PersistForOwner()
    {
        await AddUserAsync("owner");
        var created = await _service.AddAsync("owner", new TaskFormValue { Title = "Initial" });

        Assert.NotNull(await _service.UpdateAsync(
            "owner", created.Id, new TaskFormValue { Title = "Updated", Description = "Details" }));
        Assert.True(await _service.ToggleStatusAsync("owner", created.Id));
        var updated = await _service.GetByIdAsync("owner", created.Id);
        Assert.Equal("Updated", updated!.Title);
        Assert.Equal("Details", updated.Description);
        Assert.True(updated.IsCompleted);

        Assert.True(await _service.DeleteAsync("owner", created.Id));
        Assert.Null(await _service.GetByIdAsync("owner", created.Id));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_RejectsBlankTitles(string title)
    {
        await AddUserAsync("owner");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddAsync("owner", new TaskFormValue { Title = title }));
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
