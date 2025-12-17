using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TimeOfEnter.Model;
using TimeOfEnter.Repository;
using FluentValidation.Results;
using TimeOfEnter.DTO;
using System.Threading.Tasks;
using TimeOfEnter.Service;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.Common.Pagination;

namespace TimeOfEnter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterController : ControllerBase
    {
        
        private readonly IDateService dateSevice;

        public EnterController(IDateRepository dateRepository ,IDateService dateSevice)
        {
            this.dateSevice = dateSevice;
        }

        [HttpPost]
        public async Task<IActionResult> UserDate(TimeOfBookingWithoutId dto)
        { 
            await dateSevice.AddBookingAsync(dto);

            return Ok("Added Successfully");
        }
        [HttpPost("Avilable")]
        public async Task< IActionResult> CheckDate()
        {

            var matchingDates = await dateSevice.GetAvailableNowAsync();

            if (!matchingDates.Any())
                return NotFound("No available date at this time.");


          return Ok(matchingDates);

        }

        [HttpGet("AllBookingDate")]
        public async Task< ActionResult> AllBookingDate()
        {

            var bookings = await dateSevice.GetAllBookingsAsync();
            return Ok(new ApiResponse(true, new { bookings }));
        }

        [HttpGet ("Pagination")]
        public async Task<IActionResult> DatePagnation(int page=1 , int pageSize=10 )
        {
            var PageDetails= await dateSevice.GetPagedAsync(page, pageSize);

            return Ok(new ApiResponse<PageResult<AppDateDto>>( true,PageDetails)
            );
        }


    }
}
