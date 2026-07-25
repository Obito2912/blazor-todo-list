using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using blazor_todo_list.Data;

namespace blazor_todo_list.Services;

/// <summary>
/// Adds "FullName" and "Role" claims to the Identity sign-in cookie so the UI (nav menu,
/// dashboards) can show the person's name and Student/Teacher role without a DB round trip.
/// </summary>
public sealed class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor)
{
    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;

        identity.AddClaim(new Claim("FullName", user.FullName));
        identity.AddClaim(new Claim("AccountRole", user.Role.ToString()));

        return principal;
    }
}
