using Microsoft.AspNetCore.Mvc;
using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Service;

namespace TimeOfEnter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnterController(IDateService dateSevice) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UserDate(TimeOfBookingWithoutId dto)
    {
        await dateSevice.AddBookingAsync(dto);

        return Ok("Added Successfully");
    }

    [HttpPost("Avilable")]
    public async Task<IActionResult> CheckDate()
    {
        var matchingDates = await dateSevice.GetAvailableNowAsync();

        if (matchingDates.Count == 0)
            return NotFound("No available date at this time.");

        return Ok(matchingDates);
    }

    [HttpGet("AllBookingDate")]
    public async Task<IActionResult> AllBookingDate()
    {
        var bookings = await dateSevice.GetAllBookingsAsync();
        return Ok(new ApiResponse<List<AppDateDto>>(true, bookings));
    }

    [HttpGet("Pagination")]
    public async Task<IActionResult> DatePagnation(int page = 1, int pageSize = 10)
    {
        var PageDetails = await dateSevice.GetPagedAsync(page, pageSize);
        return Ok(new ApiResponse<PageResult<AppDateDto>>(true, PageDetails));
    }
}
