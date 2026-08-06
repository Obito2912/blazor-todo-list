using blazor_todo_list.Components;
using Microsoft.EntityFrameworkCore;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskListItem> AddAsync(string userId, TaskFormValue formValue)
    {
        var task = new TaskItem
        {
            Title = formValue.Title,
            Description = formValue.Description,
            DueDate = formValue.DueDate,
            UserId = userId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        return ToListItem(task);
    }

    public async Task<TaskListItem?> UpdateAsync(string userId, int taskId, TaskFormValue formValue)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return null;

        task.Title = formValue.Title;
        task.Description = formValue.Description;
        task.DueDate = formValue.DueDate;

        await _context.SaveChangesAsync();
        return ToListItem(task);
    }

    public async Task<bool> DeleteAsync(string userId, int taskId)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return false;

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
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
        return true;
    }

    private static TaskListItem ToListItem(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.IsCompleted, task.DueDate);
}