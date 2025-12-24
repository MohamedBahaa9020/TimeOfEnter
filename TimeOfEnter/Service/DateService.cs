using TimeOfEnter.Common.Pagination;
using TimeOfEnter.DTO;
using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;

namespace TimeOfEnter.Service;
public class DateService(IDateRepository dateRepository) : IDateService
{

    public async Task AddBookingAsync(TimeOfBookingWithoutId dto)
    {
        var allDates = await dateRepository.GetAllasync();

        var MatchingTime = allDates.Any(d =>
            dto.StartTime < d.EndTime &&
            dto.EndTime > d.StartTime
            );
        if (MatchingTime)
        {
            throw new Exception("This Time is Already Booking");
        }

        var booking = new Date
        {

            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true

        };

        await dateRepository.Addasync(booking);
        await dateRepository.SaveAsync();
    }

    public async Task<List<Date>> GetAvailableNowAsync()
    {
        var requestedTime = DateTime.UtcNow;
        var allDates = await dateRepository.GetAllasync();
        return [.. allDates.Where(d => requestedTime >= d.StartTime && requestedTime <= d.EndTime)];

    }
    public async Task<List<AppDateDto>> GetAllBookingsAsync()
    {
        var allDates = await dateRepository.GetAllasync();

        return [.. allDates.Select(d => new AppDateDto(d.Id, d.StartTime, d.EndTime!.Value, d.IsActive))];
    }
    public async Task<PageResult<AppDateDto>> GetPagedAsync(int page, int pageSize)
    {
        var allDates = await dateRepository.GetAllasync();
        var bookings = allDates
            .Select(d => new AppDateDto(d.Id, d.StartTime, d.EndTime, d.IsActive)).ToList();

        var skip = (page - 1) * pageSize;
        var pageDates = bookings.Skip(skip).Take(pageSize).ToList();
        var totalPages = (int)Math.Ceiling(bookings.Count / (double)pageSize);
        var countItem = bookings.Count;
        return new PageResult<AppDateDto>(page, pageSize, totalPages, countItem, bookings);
    }


}
