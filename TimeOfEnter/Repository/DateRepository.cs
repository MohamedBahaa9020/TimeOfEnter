using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TimeOfEnter.Model;

namespace TimeOfEnter.Repository
{
    public class DateRepository:IDateRepository
    {
        private readonly TestContext context;

        public DateRepository(TestContext context)
        {
            this.context = context;
        }

        public async Task< List<Date>> GetAllasync()
        {
            return await context.Dates.ToListAsync();

        }
        public async Task Addasync(Date RegisterData)
        {
           await context.AddAsync(RegisterData);

        }

        public async Task SaveAsync()
        {
           await context.SaveChangesAsync();
        }
    }
}
