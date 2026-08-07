public class TaskItem
{
    // Primary key for the TaskItem entity
    public int Id { get; set; }
    // Title of the task, cannot be null or empty
    public string Title { get; set; } = string.Empty;
    // Description of the task, can be null
    public string? Description { get; set; }
    // Indicates whether the task is completed
    public bool IsCompleted { get; set; } = false;
    // Due date of the task, can be null
    public DateTime? DueDate { get; set; }
    // Timestamp when the task was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // ID of the user who owns the task
    public string UserId { get; set; } = string.Empty;
}