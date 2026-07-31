using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? ProfileImageUrl { get; set; }

    public string FullName { get; set; } = string.Empty;
}