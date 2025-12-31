namespace TimeOfEnter.Repository;

public interface IBookingRepository
{
    Task AddBookingAsync(UserBooking user);
    Task<List<UserBooking>> GetAllBookingsAsync();
}
