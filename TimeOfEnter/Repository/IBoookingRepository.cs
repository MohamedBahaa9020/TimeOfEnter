namespace TimeOfEnter.Repository;

public interface IBookingRepository
{
    Task AddBookingAsync(UserBooking user);
    Task<List<UserBooking>> GetAllBookingsAsync(string userId);
    Task<UserBooking?> GetBookingByIdAsync(string userId, int bookingId);
    Task SaveAsync();
}
