namespace TimeOfEnter.Repository;
public class BookingRepository(TestContext context) : IBookingRepository
{
    public async Task AddBookingAsync(UserIsBooking user)
    {
        await context.UserIsBooking.AddAsync(user);
        await context.SaveChangesAsync();
    }

}
