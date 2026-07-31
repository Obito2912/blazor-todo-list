using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(string fullName, string email, string password, string role);

    Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);

    Task LogoutAsync();

    Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);

    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    Task<IdentityResult> UpdateProfileAsync(ApplicationUser user, string fullName, string email);

    Task<IdentityResult> UpdateProfileImageAsync(ApplicationUser user, string? profileImageUrl);

    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
}
