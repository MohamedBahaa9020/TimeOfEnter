using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeOfEnter.Common.Extensions;
using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Service.Interfaces;
namespace TimeOfEnter.Controllers.Dates;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class DateController(IDateService dateSevice) : ControllerBase
{
    [HttpPost("Add Dates")]
    public async Task<IActionResult> AddDate([FromBody] TimeBookingWithoutIdDto dto)
    {
        var result = await dateSevice.AddBookingAsync(dto);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }

    [HttpPost("Avilable")]
    public async Task<IActionResult> CheckDate()
    {
        var matchingDates = await dateSevice.GetAvailableNowAsync();
        return matchingDates.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }

    [HttpGet("AllBookingDate")]
    public async Task<IActionResult> AllBookingDate()
    {
        var bookings = await dateSevice.GetAllBookingsAsync();
        return bookings.Match(
            success => Ok(new ApiResponse<List<AppDateDto>>(true, success)),
            errors => this.Problem(errors));
    }

    [HttpGet("Pagination")]
    public async Task<IActionResult> DatePagnation(int page = 1, int pageSize = 10)
    {
        var pageDetailsResult = await dateSevice.GetPagedAsync(page, pageSize);
        return pageDetailsResult.Match(
            success => Ok(new ApiResponse<PageResult<AppDateDto>>(true, success)),
            errors => this.Problem(errors));
    }

    [HttpPost("Booking")]
    public async Task<IActionResult> BookDate([FromBody] int dateId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await dateSevice.BookAvilableDate(userId!, dateId);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
    [HttpGet("UserBookings")]
    public async Task<IActionResult> GetUserBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await dateSevice.GetAllUserBookings(userId!);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
    [HttpDelete("CancelBooking")]
    public async Task<IActionResult> CancelBooking([FromBody] int bookingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await dateSevice.CancelBookingAsync(userId!, bookingId);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
    [HttpDelete("CancelAllBookings")]
    public async Task<IActionResult> CancelAllUserBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await dateSevice.CancelAllBooking(userId!);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
}
