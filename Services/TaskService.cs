using blazor_todo_list.Components;
using Microsoft.EntityFrameworkCore;

public class TaskService : ITaskService
{
    // Dependency injection for the database context and logger
    private readonly AppDbContext _context;
    // Add a logger to log important events and errors
    private readonly ILogger<TaskService> _logger;
    // Constructor injection for AppDbContext and ILogger
    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        // Initialize the injected services
        _context = context;
        _logger = logger;
    }

    public async Task<TaskListItem> AddAsync(string userId, TaskFormValue formValue)
    {
        ValidateFormValue(formValue);

        var task = new TaskItem
        {
            Title = formValue.Title.Trim(),
            Description = formValue.Description,
            DueDate = formValue.DueDate,
            UserId = userId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task added successfully for user {UserId} with task id {TaskId}.", userId, task.Id);
        return ToListItem(task);
    }

    public async Task<TaskListItem?> UpdateAsync(string userId, int taskId, TaskFormValue formValue)
    {
        ValidateFormValue(formValue);

        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return null;

        task.Title = formValue.Title.Trim();
        task.Description = formValue.Description;
        task.DueDate = formValue.DueDate;

        // Save changes to the database
        await _context.SaveChangesAsync();
        // Log the successful update of the task
        _logger.LogInformation("Task updated successfully for user {UserId} with task id {TaskId}.", userId, task.Id);
        return ToListItem(task);
    }

    public async Task<bool> DeleteAsync(string userId, int taskId)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return false;

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
        // Log the successful deletion of the task
        _logger.LogInformation("Task deleted successfully for user {UserId} with task id {TaskId}.", userId, task.Id);
        return true;
    }

    public async Task<TaskListItem?> GetByIdAsync(string userId, int taskId)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        return task is null ? null : ToListItem(task);
    }

    public async Task<List<TaskListItem>> GetAllForUserAsync(string userId)
    {
        return await _context.TaskItems
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskListItem(t.Id, t.Title, t.Description, t.IsCompleted, t.DueDate))
            .ToListAsync();
    }

    public async Task<List<TaskListItem>> SearchAsync(string userId, string searchTerm)
    {
        var term = searchTerm?.Trim() ?? string.Empty;

        return await _context.TaskItems
            .Where(t => t.UserId == userId && EF.Functions.Like(t.Title, $"%{term}%"))
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskListItem(t.Id, t.Title, t.Description, t.IsCompleted, t.DueDate))
            .ToListAsync();
    }

    public async Task<List<TaskListItem>> FilterByStatusAsync(string userId, TaskFilter filter)
    {
        var query = _context.TaskItems.Where(t => t.UserId == userId);

        query = filter switch
        {
            TaskFilter.Pending => query.Where(t => !t.IsCompleted),
            TaskFilter.Completed => query.Where(t => t.IsCompleted),
            _ => query
        };

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskListItem(t.Id, t.Title, t.Description, t.IsCompleted, t.DueDate))
            .ToListAsync();
    }

    public async Task<bool> ToggleStatusAsync(string userId, int taskId)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return false;

        task.IsCompleted = !task.IsCompleted;
        await _context.SaveChangesAsync();
        // Log the successful toggling of the task status
        _logger.LogInformation("Task status toggled successfully for user {UserId} with task id {TaskId}. New status: {IsCompleted}", userId, task.Id, task.IsCompleted);
        return true;
    }

    private static void ValidateFormValue(TaskFormValue formValue)
    {
        if (string.IsNullOrWhiteSpace(formValue.Title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(formValue));
        }

        if (formValue.DueDate is { } dueDate && dueDate.Date < DateTime.Today)
        {
            throw new ArgumentException("Due date cannot be in the past.", nameof(formValue));
        }
    }

    private static TaskListItem ToListItem(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.IsCompleted, task.DueDate);
}