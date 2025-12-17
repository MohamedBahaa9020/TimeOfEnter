using Microsoft.EntityFrameworkCore;

namespace TimeOfEnter.Model
{
    public class TestContext:DbContext
    {
        public DbSet<Date> Dates { get; set; }
        

        public TestContext(DbContextOptions options):base(options)
        {
            
        }
       
    }
}
