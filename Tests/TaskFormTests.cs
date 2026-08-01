using System.ComponentModel.DataAnnotations;
using blazor_todo_list.Components;

public sealed class TaskFormTests
{
    [Fact]
    public void TaskForm_RequiresTitle()
    {
        var model = new TaskFormValue();
        var results = Validate(model);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(TaskFormValue.Title)));
    }

    [Fact]
    public void TaskForm_RejectsOversizedFields()
    {
        var model = new TaskFormValue
        {
            Title = new string('t', 121),
            Description = new string('d', 1001)
        };

        Assert.Equal(2, Validate(model).Count);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }
}
