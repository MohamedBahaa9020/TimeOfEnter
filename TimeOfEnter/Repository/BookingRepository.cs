using TimeOfEnter.Database;
namespace TimeOfEnter.Repository;

public class BookingRepository(DateContext context) : IBookingRepository
{
    public async Task AddBookingAsync(UserBooking user)
    {
        await context.UserIsBooking.AddAsync(user);
        await context.SaveChangesAsync();
    }
}
