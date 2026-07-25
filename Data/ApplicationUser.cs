using Microsoft.AspNetCore.Identity;

namespace blazor_todo_list.Data;

/// <summary>
/// The account role chosen at registration. Drives which parts of My Quest a user sees
/// (a Student manages their own quests; a Teacher can assign and review quests for a class).
/// </summary>
public enum AccountRole
{
    Student,
    Teacher
}

/// <summary>
/// Application-specific identity user. Extends the default Identity user with the
/// profile fields My Quest needs: a display name and the Student/Teacher account role.
/// </summary>
public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public AccountRole Role { get; set; } = AccountRole.Student;
}
