namespace TimeOfEnter.Repository;

public interface IBookingRepository
{
    Task AddBookingAsync(UserBooking user);
}
