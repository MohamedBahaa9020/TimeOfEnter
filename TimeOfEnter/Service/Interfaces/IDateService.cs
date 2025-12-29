using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;

namespace TimeOfEnter.Service.Interfaces;
public interface IDateService
{
    Task AddBookingAsync(TimeOfBookingWithoutId dto);
    Task<List<Date>> GetAvailableNowAsync();
    Task<List<AppDateDto>> GetAllBookingsAsync();
    Task<BookingDateResponses> BookAvilableDate(string? userId);
    Task<PageResult<AppDateDto>> GetPagedAsync(int page, int pageSize);

}
