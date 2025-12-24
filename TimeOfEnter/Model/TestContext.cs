using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TimeOfEnter.Model;
public class TestContext(DbContextOptions<TestContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Date> Dates { get; set; }
}
