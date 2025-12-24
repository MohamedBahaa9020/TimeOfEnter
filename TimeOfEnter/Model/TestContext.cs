using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TimeOfEnter.Model
{
    public class TestContext : IdentityDbContext<AppUser>
    {
        public DbSet<Date> Dates { get; set; }


        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {

        }

    }
}
