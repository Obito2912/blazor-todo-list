using Microsoft.AspNetCore.Identity;
using blazor_todo_list.Data;

namespace blazor_todo_list.Services;

public sealed class AccountService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IAccountService
{
    public async Task<IdentityResult> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
        }

        return result;
    }

    public Task<SignInResult> LoginAsync(string email, string password, bool rememberMe) =>
        signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

    public Task LogoutAsync() => signInManager.SignOutAsync();

    public async Task<IdentityResult> UpdateProfileAsync(ApplicationUser user, ProfileUpdateRequest request)
    {
        user.FullName = request.FullName;
        user.Role = request.Role;
        return await userManager.UpdateAsync(user);
    }

    public Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword) =>
        userManager.ChangePasswordAsync(user, currentPassword, newPassword);

    public Task<ApplicationUser?> FindByEmailAsync(string email) => userManager.FindByEmailAsync(email);
}
