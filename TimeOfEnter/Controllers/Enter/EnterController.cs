using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeOfEnter.Common.Pagination;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Service.Interfaces;

namespace TimeOfEnter.Controllers.Enter;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class EnterController(IDateService dateSevice) : ControllerBase
{
    //[Authorize(Roles = "Admin")]
    [HttpPost("Add Dates")]
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

    [HttpPost("Booking")]
    public async Task<IActionResult> BookDate()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await dateSevice.BookAvilableDate(userId);
        if(!result.IsActive)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
