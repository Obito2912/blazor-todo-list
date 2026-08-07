using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    // Add a logger to log important events and errors
    private readonly ILogger<AccountService> _logger;
    // Constructor injection for UserManager, SignInManager, and ILogger
    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountService> logger)
    {
        // Initialize the injected services
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IdentityResult> RegisterAsync(string fullName, string email, string password, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return result;
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return result;
    }

    public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return SignInResult.Failed;
        }

        return await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return await _userManager.GetUserAsync(principal);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> UpdateProfileAsync(ApplicationUser user, string fullName, string email)
    {
        user.FullName = fullName;

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            // here both email and username need updating together so login wont break after changing one of them
            var setEmailResult = await _userManager.SetEmailAsync(user, email);
            if (!setEmailResult.Succeeded)
            {
                return setEmailResult;
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                return setUserNameResult;
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (updateResult.Succeeded)
        {
            // Refresh the sign-in to update the user's claims and authentication cookie
            await _signInManager.RefreshSignInAsync(user);
            // Log the successful profile update
            _logger.LogInformation("User profile updated successfully for user {UserId}.", user.Id);
        }

        return updateResult;
    }

    public async Task<IdentityResult> UpdateProfileImageAsync(ApplicationUser user, string? profileImageUrl)
    {
        user.ProfileImageUrl = string.IsNullOrWhiteSpace(profileImageUrl) ? null : profileImageUrl.Trim();
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            // Refresh the sign-in to update the user's claims and authentication cookie
            await _signInManager.RefreshSignInAsync(user);
            // Log the successful profile image update
            _logger.LogInformation("User profile image updated successfully for user {UserId}.", user.Id);
        }

        return result;
    }

    // Using UserManager to handle hash comparison and new password validation against our rules
    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            // Refresh the sign-in to update the user's claims and authentication cookie
            await _signInManager.RefreshSignInAsync(user);
            // Log the successful password change
            _logger.LogInformation("User password changed successfully for user {UserId}.", user.Id);
        }

        return result;
    }
}
