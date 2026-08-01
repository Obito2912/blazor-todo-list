using Microsoft.EntityFrameworkCore;

public sealed class TaskService : ITaskService
{
    private readonly AppDbContext _dbContext;

    public TaskService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(string userId, CancellationToken cancellationToken = default)
    {
        EnsureUserId(userId);

        return await _dbContext.TaskItems
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .OrderBy(task => task.IsCompleted)
            .ThenBy(task => task.DueDate == null)
            .ThenBy(task => task.DueDate)
            .ThenByDescending(task => task.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetTaskAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        EnsureUserId(userId);
        return await _dbContext.TaskItems.AsNoTracking()
            .SingleOrDefaultAsync(task => task.Id == id && task.UserId == userId, cancellationToken);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        EnsureUserId(userId);
        Validate(task);

        var entity = new TaskItem
        {
            Title = task.Title.Trim(),
            Description = NormalizeDescription(task.Description),
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        _dbContext.TaskItems.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        EnsureUserId(userId);
        Validate(task);

        var entity = await _dbContext.TaskItems
            .SingleOrDefaultAsync(item => item.Id == task.Id && item.UserId == userId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Title = task.Title.Trim();
        entity.Description = NormalizeDescription(task.Description);
        entity.DueDate = task.DueDate;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        EnsureUserId(userId);
        var entity = await _dbContext.TaskItems
            .SingleOrDefaultAsync(task => task.Id == id && task.UserId == userId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.TaskItems.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetCompletionAsync(int id, bool isCompleted, string userId, CancellationToken cancellationToken = default)
    {
        EnsureUserId(userId);
        var entity = await _dbContext.TaskItems
            .SingleOrDefaultAsync(task => task.Id == id && task.UserId == userId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsCompleted = isCompleted;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void EnsureUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("A user ID is required.", nameof(userId));
        }
    }

    private static void Validate(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            throw new ArgumentException("A task title is required.", nameof(task));
        }

        if (task.Title.Trim().Length > 120)
        {
            throw new ArgumentException("The task title cannot exceed 120 characters.", nameof(task));
        }

        if (task.Description?.Length > 1000)
        {
            throw new ArgumentException("The task description cannot exceed 1,000 characters.", nameof(task));
        }
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
