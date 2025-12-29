using ErrorOr;
using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
namespace TimeOfEnter.Service.Interfaces;

public interface IDateService
{
    Task<ErrorOr<Success>> AddBookingAsync(TimeBookingWithoutIdDto dto);
    Task<ErrorOr<List<Date>>> GetAvailableNowAsync();
    Task<ErrorOr<List<AppDateDto>>> GetAllBookingsAsync();
    Task<ErrorOr<BookingDateResponse>> BookAvilableDate(string? userId);
    Task<ErrorOr<PageResult<AppDateDto>>> GetPagedAsync(int page, int pageSize);
    Task DeleteNoneActiveDate();
    Task UpdateDateActivation();
}
