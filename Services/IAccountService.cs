using Microsoft.AspNetCore.Identity;
using blazor_todo_list.Data;

namespace blazor_todo_list.Services;

public sealed record RegisterRequest(string FullName, string Email, string Password, AccountRole Role);

public sealed record ProfileUpdateRequest(string FullName, AccountRole Role);

/// <summary>
/// Wraps ASP.NET Core Identity's UserManager/SignInManager behind a small, testable
/// surface for the account pages (register, login, logout, profile edit).
/// </summary>
public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterRequest request);

    Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);

    Task LogoutAsync();

    Task<IdentityResult> UpdateProfileAsync(ApplicationUser user, ProfileUpdateRequest request);

    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);

    Task<ApplicationUser?> FindByEmailAsync(string email);
}
