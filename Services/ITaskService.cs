public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(string userId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetTaskAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateTaskAsync(TaskItem task, string userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateTaskAsync(TaskItem task, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<bool> SetCompletionAsync(int id, bool isCompleted, string userId, CancellationToken cancellationToken = default);
}
