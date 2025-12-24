using TimeOfEnter.Common.Pagination;
using TimeOfEnter.DTO;

namespace TimeOfEnter.Service.Interfaces;
public interface IDateService
{
    Task AddBookingAsync(TimeOfBookingWithoutId dto);
    Task<List<Date>> GetAvailableNowAsync();
    Task<List<AppDateDto>> GetAllBookingsAsync();
    Task<PageResult<AppDateDto>> GetPagedAsync(int page, int pageSize);

}
