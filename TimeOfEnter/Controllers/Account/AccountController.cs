using Microsoft.AspNetCore.Mvc;
using TimeOfEnter.Common.Extensions;
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

        return result.Match(
        success => Ok(success),
        errors => this.Problem(errors));
    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await accountService.LoginAsync(loginDto);

        return token.Match(
           success => Ok(success),
           errors => this.Problem(errors));
    }

    [HttpPost("Addrole")]
    public async Task<IActionResult> AddNewUserRole(AddRoleDto add)
    {
        var result = await accountService.AddRoleAsync(add);
        return result.Match(
            _ => Ok("Role added successfully"),
            errors => this.Problem(errors));
    }

    [HttpPost("GetToken")]
    public async Task<IActionResult> GetRefreshAndAccsessToken([FromBody] string refreshToken)
    {
        var result = await accountService.RefreshTokenAsync(refreshToken);

        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] string token)
    {
        var refreshToken = await accountService.RevokeTokenAsync(token);

        return refreshToken.Match(
          _ => Ok("Revoked Successfully"),
          errors => this.Problem(errors));
    }

    // TODO: get profile data
    // TODO: update profile data (email, picture)
    // TODO: delete account (soft delete)

    // TODO: upload profile picture 
    // TODO: delete profile picture

    // ------------------------------------------------------------
    // Search: ef interceptor
    // Search: Auditable Entity
}
