namespace TimeOfEnter.Repository;
public interface IBookingRepository
{
    Task AddBookingAsync(UserIsBooking user);
}
