using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;

namespace TimeOfEnter.Service;
public class DateService(IDateRepository dateRepository, IBookingRepository bookingRepository) : IDateService
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
            IsActive = true,
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

    public async Task<BookingDateResponses> BookAvilableDate(string? userId)
    {
        var matchingDates = await GetAvailableNowAsync();
        if (matchingDates == null||matchingDates.Count == 0)
            return new BookingDateResponses
                (IsActive:false,
                 Message:"No available date at this time.",
                 StartTime:null,
                 EndTime:null);
        
        
        var bookedDate = matchingDates.First();
        var booking = new UserIsBooking
        {
            UserId = userId,
            StartTime = bookedDate.StartTime,
            EndTime = bookedDate.EndTime
        };
        await bookingRepository.AddBookingAsync(booking);
        return new BookingDateResponses
               (IsActive:true,
                Message: "Booked Successfully",
                StartTime: bookedDate.StartTime,
                EndTime: bookedDate.EndTime);

    }
}
