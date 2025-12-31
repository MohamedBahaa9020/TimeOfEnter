using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace TimeOfEnter.Database;

public class DateContext(DbContextOptions<DateContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Date> Dates { get; set; }
    public DbSet<UserBooking> UserBooking { get; set; }
}
