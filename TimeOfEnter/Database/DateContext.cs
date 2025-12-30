using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace TimeOfEnter.Database;

public class DateContext(DbContextOptions<DateContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Date> Dates { get; set; } = default!;
    public DbSet<UserBooking> UserIsBooking { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(DateContext).Assembly);
    }
}
