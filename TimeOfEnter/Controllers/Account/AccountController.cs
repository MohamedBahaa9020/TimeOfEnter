using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            success => Ok(success),
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
          success => Ok(success),
          errors => this.Problem(errors));
    }
    [HttpGet("AllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await accountService.AllUsersAsync();
        return users.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
    [Authorize]
    [HttpGet("UserProfile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = await accountService.GetProfileData(userId!);
        return userProfile.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
    [Authorize]
    [HttpPut("UpdateEmail")]
    public async Task<IActionResult> UpdateData([FromForm] UpdateDataDto dataDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await accountService.UpdateProfileData(userId!, dataDto);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors));
    }
    [Authorize]
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUserProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await accountService.DeleteProfileData(userId!);
        return result.Match(
            success => Ok(success),
            errors => this.Problem(errors)
            );
    }
}
