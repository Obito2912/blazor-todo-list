using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
    {
        _logger = logger;
    }
    // Logger for logging errors
    private readonly ILogger<AppDbContext> _logger;

    public DbSet<TaskItem> TaskItems { get; set; } = null!;

    // Configure the model relationships and constraints
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure the relationship between TaskItem and ApplicationUser
        modelBuilder.Entity<TaskItem>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Ensure that the NormalizedEmail field in ApplicationUser is unique
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.NormalizedEmail)
            .IsUnique();
    }
    // Override SaveChangesAsync to log errors during database operations
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log the error using the injected logger
            _logger.LogError(ex, "An error occurred while saving changes to the database.");
            throw;
        }
    }
}