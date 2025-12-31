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
    public async Task<List<UserBooking>> GetAllBookingsAsync()
    {
        return await context.UserBooking.ToListAsync();
    }
}
