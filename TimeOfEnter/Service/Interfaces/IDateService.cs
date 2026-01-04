using ErrorOr;
using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
namespace TimeOfEnter.Service.Interfaces;

public interface IDateService
{
    Task<ErrorOr<Success>> AddBookingAsync(TimeBookingWithoutIdDto dto);
    Task<ErrorOr<List<AppDateDto>>> GetAvailableNowAsync();
    Task<ErrorOr<List<AppDateDto>>> GetAllBookingsAsync();
    Task<ErrorOr<BookingDateResponse>> BookAvilableDate(string userId, int dateId);
    Task<ErrorOr<List<UserBookingsResponse>>> GetAllUserBookings(string userId);
    Task<ErrorOr<PageResult<AppDateDto>>> GetPagedAsync(int page, int pageSize);
    Task<ErrorOr<CancelResponse>> CancelBookingAsync(string userId, int bookingId);
    Task<ErrorOr<MessageResponse>> CancelAllBooking(string userId);
    Task DeleteNoneActiveDate();
    Task UpdateDateActivation();
}
