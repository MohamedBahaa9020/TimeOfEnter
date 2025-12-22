using TimeOfEnter.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Helper;
using TimeOfEnter.Service;

namespace TimeOfEnter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService, IOptions<JWT> jwt) : ControllerBase
{
    private readonly JWT jwt = jwt.Value;

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
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

        if (token != null)
        {

            return Ok(token);
        }

        return BadRequest("Email or Password Invalid");

    }
    
    [HttpPost("Addrole")]
    public async Task<IActionResult> AddNewUserRole(AddRole add)
    {
        var result = await accountService.AddRoleAsync(add);
        if (result.IsNullOrEmpty())
            return BadRequest("Failed to add role to user");
        return Ok(result);

    }

    [HttpGet("GetToken")]
    public async Task<IActionResult> GetRefreshAndAccsessToken([FromBody] string refreshToken)
    {
        var result = await accountService.RefreshTokenAsync(refreshToken);

        if (!result.IsAuthenticated) { 
        
        return BadRequest(result);
        }
        return Ok(result);

    }

    [HttpPost("revokeToken")]
    public async Task<IActionResult>RevokeRefreshToken([FromBody] string token)
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

    //private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
    //{
    //    var cookieOption = new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Expires = expires
    //    };
    //    Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
    //}

}
