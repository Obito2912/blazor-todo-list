using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountService> _logger;
    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IdentityResult> RegisterAsync(string fullName, string email, string password, string role)
    {
        if (role is not ("Student" or "Teacher"))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidRole",
                Description = "Choose either the Student or Teacher role."
            });
        }

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

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return roleResult;
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
            await _signInManager.RefreshSignInAsync(user);
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
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User profile image updated successfully for user {UserId}.", user.Id);
        }

        return result;
    }

    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User password changed successfully for user {UserId}.", user.Id);
        }

        return result;
    }
}
