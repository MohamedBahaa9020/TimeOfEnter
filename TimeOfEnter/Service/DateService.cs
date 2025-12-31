using ErrorOr;
using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Errors;
using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;
namespace TimeOfEnter.Service;

public class DateService(IDateRepository dateRepository, IBookingRepository bookingRepository) : IDateService
{
    public async Task<ErrorOr<Success>> AddBookingAsync(TimeBookingWithoutIdDto dto)
    {
        var allDates = await dateRepository.GetAllasync();
        var MatchingTime = allDates.Any(d =>
            dto.StartTime < d.EndTime &&
            dto.EndTime > d.StartTime);
        if (MatchingTime)
        {
            return DateErrors.TimeAlreadyBooking;
        }

        var booking = new Date
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true,
        };

        await dateRepository.Addasync(booking);
        await dateRepository.SaveAsync();
        return Result.Success;
    }

    public async Task<ErrorOr<List<AppDateDto>>> GetAvailableNowAsync()
    {
        var requestedTime = DateTime.UtcNow;
        var allDates = await dateRepository.GetAllasync();
        var availableDates = allDates.Where(d => requestedTime < d.StartTime).ToList();
        if (availableDates == null || availableDates.Count == 0)
        {
            return DateErrors.NoDatesAvailable;
        }
        var result = availableDates
            .Select(d => new AppDateDto(d.Id, d.StartTime, d.EndTime, d.IsActive))
            .ToList();
        return result;
    }
    public async Task<ErrorOr<List<AppDateDto>>> GetAllBookingsAsync()
    {
        var allDates = await dateRepository.GetAllasync();
        if (allDates.Count == 0)
        {
            return DateErrors.InvalidDate;
        }

        var result = allDates
            .Select(d => new AppDateDto(d.Id, d.StartTime, d.EndTime, d.IsActive))
            .ToList();

        return result;
    }
    public async Task<ErrorOr<PageResult<AppDateDto>>> GetPagedAsync(int page, int pageSize)
    {
        var allDates = await dateRepository.GetAllasync();
        var bookings = allDates
            .Select(d => new AppDateDto(d.Id, d.StartTime, d.EndTime, d.IsActive)).ToList();
        if (bookings.Count == 0)
        {
            return DateErrors.InvalidDate;
        }
        var skip = (page - 1) * pageSize;
        var pageDates = bookings.Skip(skip).Take(pageSize).ToList();
        var totalPages = (int)Math.Ceiling(bookings.Count / (double)pageSize);
        var countItem = bookings.Count;
        return new PageResult<AppDateDto>(page, pageSize, totalPages, countItem, bookings);
    }

    public async Task<ErrorOr<BookingDateResponse>> BookAvilableDate(string userId, int dateId)
    {
        var matchingDatesResult = await GetAvailableNowAsync();
        if (matchingDatesResult.IsError)
        {
            return matchingDatesResult.Errors;
        }

        if (string.IsNullOrEmpty(userId))
        {
            return DateErrors.UserRequired;
        }
        var bookedDate = matchingDatesResult.Value.FirstOrDefault(d => d.Id == dateId);

        if (bookedDate == null)
        {
            return DateErrors.InvalidDate;
        }

        var booking = new UserBooking
        {
            UserId = userId,
            DateId = dateId,
        };
        await bookingRepository.AddBookingAsync(booking);

        return new BookingDateResponse
               (IsActive: true,
                Message: "Booked Successfully",
                StartTime: bookedDate.StartTime,
                EndTime: bookedDate.EndTime);

    }
    public async Task<ErrorOr<List<UserBookingsResponse>>> GetAllUserBookings(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return DateErrors.UserRequired;
        }

        var bookings = await bookingRepository.GetAllBookingsAsync();

        if (bookings.Count == 0)
        {
            return DateErrors.NoBookingsFound;
        }

        var userBookings = bookings
            .Where(b => b.UserId == userId)
            .Select(b => new UserBookingsResponse(
                b.Date.IsActive,
                b.Date.StartTime,
                b.Date.EndTime
            ))
            .ToList();
        return userBookings;
    }
    public async Task DeleteNoneActiveDate()
    {
        var dates = await dateRepository.GetAllasync();

        var noneActiveDates = dates.Where(d => d.IsActive == false).ToList();

        await dateRepository.DeleteRangeAsync(noneActiveDates);
    }
    public async Task UpdateDateActivation()
    {
        var allDates = await dateRepository.GetAllasync();
        if (allDates.Any(d => d.EndTime <= DateTime.UtcNow))
        {
            allDates.ForEach(d => d.IsActive = false);
            await dateRepository.SaveAsync();
        }
    }
}
