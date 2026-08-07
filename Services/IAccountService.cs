using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

// Returns IdentityResult type (not bool) for anything that can fail, so the UI can show the actual reason, not a of a generic error.
public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(string fullName, string email, string password, string role);

    Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);

    Task LogoutAsync();

    // this is meant to be called from a Razor component's AuthenticationState to save callers from having to pull the id out themselves every time.
    Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);

    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    // Email and username are kept in sync 
    Task<IdentityResult> UpdateProfileAsync(ApplicationUser user, string fullName, string email);

    Task<IdentityResult> UpdateProfileImageAsync(ApplicationUser user, string? profileImageUrl);

    // Requires currentPassword on purpose for security reasons
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
}
