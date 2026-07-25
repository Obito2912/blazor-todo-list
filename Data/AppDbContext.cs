using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace blazor_todo_list.Data;

/// <summary>
/// EF Core database context for My Quest. Built on IdentityDbContext so account
/// (register/login/roles) tables come for free alongside the app's own tables.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(u => u.FullName).HasMaxLength(120).IsRequired();
            user.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });
    }
}
