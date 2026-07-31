using System.ComponentModel.DataAnnotations;

namespace blazor_todo_list.Components;

public sealed class RegisterFormValue
{
    [Required(ErrorMessage = "Choose how you're joining.")]
    public string Role { get; set; } = "Student";

    [Required(ErrorMessage = "Enter your full name.")]
    [StringLength(100, ErrorMessage = "Keep your name under 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your email address.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter a password.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
