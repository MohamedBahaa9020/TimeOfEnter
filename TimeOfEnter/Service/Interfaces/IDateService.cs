using TimeOfEnter.Common.Pagination;
using TimeOfEnter.DTO;
using TimeOfEnter.Model;



namespace TimeOfEnter.Service
{
    public interface IDateService
    {
        Task AddBookingAsync(TimeOfBookingWithoutId dto);
        Task<List<Date>> GetAvailableNowAsync();
        Task<List<AppDateDto>> GetAllBookingsAsync();
        Task<PageResult<AppDateDto>> GetPagedAsync(int page, int pageSize);
    }
}
