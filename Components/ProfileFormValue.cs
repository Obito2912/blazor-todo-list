using System.ComponentModel.DataAnnotations;

namespace blazor_todo_list.Components;

public sealed class ProfileFormValue
{
    [Required(ErrorMessage = "Enter your full name.")]
    [StringLength(100, ErrorMessage = "Keep your name under 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your email address.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Keep the image URL under 300 characters.")]
    [Url(ErrorMessage = "Enter a valid URL.")]
    public string? ProfileImageUrl { get; set; }
}

public sealed class ChangePasswordFormValue
{
    [Required(ErrorMessage = "Enter your current password.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter a new password.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your new password.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
