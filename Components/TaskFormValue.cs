using System.ComponentModel.DataAnnotations;

namespace blazor_todo_list.Components;

public sealed class TaskFormValue
{
    [Required(ErrorMessage = "Enter a title.")]
    [StringLength(120, ErrorMessage = "Keep the title under 120 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Keep the description under 1,000 characters.")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}
