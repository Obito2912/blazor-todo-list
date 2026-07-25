using Microsoft.AspNetCore.Identity;
using blazor_todo_list.Data;

namespace blazor_todo_list.Services;

/// <summary>
/// Fetches the signed-in ApplicationUser for the current HttpContext, or signs the
/// person out and sends them back to login if their account somehow no longer exists
/// (e.g. it was deleted while their cookie was still valid).
/// </summary>
internal sealed class IdentityUserAccessor(
    UserManager<ApplicationUser> userManager,
    IdentityRedirectManager redirectManager)
{
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is null)
        {
            redirectManager.RedirectTo("/logout");
        }

        return user!;
    }
}
