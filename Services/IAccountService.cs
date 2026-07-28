using Microsoft.AspNetCore.Identity;

public interface IAccountService
{
    Task<IdentityResult> UpdateUsernameAsync(string userId, string newUsername);

    Task<IdentityResult> UpdatePasswordAsync(string userId, string currentPassword, string newPassword);

    Task<IdentityResult> UpdateProfileImageAsync(string userId, string imageUrl);
}