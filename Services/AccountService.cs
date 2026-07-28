using Microsoft.AspNetCore.Identity;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> UpdateUsernameAsync(string userId, string newUsername)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found."
            });
        }

        var setUsernameResult = await _userManager.SetUserNameAsync(user, newUsername);
        if (!setUsernameResult.Succeeded)
        {
            return setUsernameResult;
        }

        // Keep this retunr method of email in sync only if username will be treated as email 
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> UpdatePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found."
            });
        }

        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<IdentityResult> UpdateProfileImageAsync(string userId, string imageUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found."
            });
        }

        user.ProfileImageUrl = imageUrl;
        return await _userManager.UpdateAsync(user);
    }
}