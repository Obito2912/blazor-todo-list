using System.ComponentModel.DataAnnotations;

namespace blazor_todo_list.Components;

public sealed class LoginFormValue
{
    [Required(ErrorMessage = "Enter your email address.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your password.")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
