using Microsoft.AspNetCore.Mvc;
using TimeOfEnter.DTO;
using TimeOfEnter.Service.Interfaces;

namespace TimeOfEnter.Controllers.Account;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {

        var result = await accountService.RegisterAsync(registerDto);

        if (!result.IsAuthenticated)
        {
            return BadRequest(result.Massage);

        }
        return Ok(result);

    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await accountService.LoginAsync(loginDto);

        if (token.IsAuthenticated)
        {

            return Ok(token);
        }

        return Unauthorized(token);

    }

    [HttpPost("Addrole")]
    public async Task<IActionResult> AddNewUserRole(AddRole add)
    {
        var result = await accountService.AddRoleAsync(add);
        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPost("GetToken")]
    public async Task<IActionResult> GetRefreshAndAccsessToken([FromBody] string refreshToken)
    {
        var result = await accountService.RefreshTokenAsync(refreshToken);

        if (!result.IsAuthenticated)
        {

            return BadRequest(result);
        }
        return Ok(result);

    }

    [HttpPost("Logout")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] string token)
    {
        var refreshToken = await accountService.RevokeTokenAsync(token);

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is required");
        }

        if (!refreshToken)
        {
            return BadRequest("Token is invalid!");
        }
        return Ok("Revoked Successfully");
    }
}
