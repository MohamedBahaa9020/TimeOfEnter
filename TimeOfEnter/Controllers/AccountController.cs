using AuthenticationProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeOfEnter.DTO;

namespace AuthenticationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<AppUser> userManager , IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid) {
            AppUser appUser = new AppUser();

                appUser.UserName = registerDto.UserName;
                appUser.Email = registerDto.Email;
                
                IdentityResult result = await userManager.CreateAsync(appUser,registerDto.Password);

                if (result.Succeeded) {
                    return Ok("Created");
                
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(loginDto.Email);

                if (appUser != null) { 
                
                    bool found = await userManager.CheckPasswordAsync(appUser, loginDto.Password);

                    if (found) { 

                        List<Claim>claims = new List<Claim>();

                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier,appUser.Id));
                        claims.Add(new Claim(ClaimTypes.Email, appUser.Email));

                        var userRole = await userManager.GetRolesAsync(appUser);

                        foreach (var item in userRole)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, item));
                        }

                        var signKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes("fkagjsjjgjfjakkj1356563#fnkldfds"));

                        var signingCred = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

                        JwtSecurityToken jwtSecurity = new JwtSecurityToken(
                            audience: configuration["JWT:AudienceIP"],
                            issuer: configuration["JWT:IssuerIP"],
                            expires:DateTime.Now.AddDays(1),
                            claims:claims,
                            signingCredentials: signingCred 

                            );

                        return Ok(new
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurity),
                            Expiration = DateTime.Now.AddHours(1)
                        });




                    
                    }
                
                }

                ModelState.AddModelError("Email", "Email OR Password  Invalid");
            }
            return BadRequest(ModelState);

        }
    }
}
