using blazor_todo_list.Components;

public sealed class TaskListTests
{
    [Fact]
    public void TaskListItem_PreservesPersistenceValues()
    {
        var dueDate = new DateTime(2026, 8, 10);
        var item = new TaskListItem(42, "Ship project", "Verify it", true, dueDate);

        Assert.Equal(42, item.Id);
        Assert.True(item.IsCompleted);
        Assert.Equal(dueDate, item.DueDate);
    }
}
