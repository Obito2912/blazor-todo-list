namespace blazor_todo_list.Components;

public sealed record TaskListItem(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate);
