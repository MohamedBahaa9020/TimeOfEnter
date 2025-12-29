using Microsoft.EntityFrameworkCore;
using TimeOfEnter.Database;
namespace TimeOfEnter.Repository;

public class DateRepository(DateContext context) : IDateRepository
{
    public async Task<List<Date>> GetAllasync()
    {
        return await context.Dates.ToListAsync();
    }
    public async Task Addasync(Date RegisterData)
    {
        await context.AddAsync(RegisterData);
    }
    public async Task DeleteRangeAsync(List<Date> dates)
    {
        context.Dates.RemoveRange(dates);
        await SaveAsync();
    }
    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}
