using blazor_todo_list.Components;

// Methods depends on userId to keep ownership inside the service layer
public interface ITaskService
{
    Task<TaskListItem> AddAsync(string userId, TaskFormValue formValue);

    // Returns null instead of throwing if the task isnt found or doesnt belong to the user
    Task<TaskListItem?> UpdateAsync(string userId, int taskId, TaskFormValue formValue);

    Task<bool> DeleteAsync(string userId, int taskId);

    Task<TaskListItem?> GetByIdAsync(string userId, int taskId);

    Task<List<TaskListItem>> GetAllForUserAsync(string userId);

    Task<List<TaskListItem>> SearchAsync(string userId, string searchTerm);

    Task<List<TaskListItem>> FilterByStatusAsync(string userId, TaskFilter filter);

    Task<bool> ToggleStatusAsync(string userId, int taskId);
}