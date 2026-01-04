using Microsoft.EntityFrameworkCore;
using TimeOfEnter.Database;
namespace TimeOfEnter.Repository;

public class BookingRepository(DateContext context) : IBookingRepository
{
    public async Task AddBookingAsync(UserBooking user)
    {
        await context.UserBooking.AddAsync(user);
        await context.SaveChangesAsync();
    }
    public async Task<List<UserBooking>> GetAllBookingsAsync(string userId)
    {
        return await context.UserBooking
            .Include(b => b.Date)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }
    public async Task<UserBooking?> GetBookingByIdAsync(string userId, int bookingId)
    {
        return await context.UserBooking
            .Include(b => b.Date)
            .Where(b => b.UserId == userId &&
                        b.DateId == bookingId)
            .FirstOrDefaultAsync();
    }
    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}
