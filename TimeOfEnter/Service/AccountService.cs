using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeOfEnter.DTO;
using TimeOfEnter.Helper;
using TimeOfEnter.Model;

namespace TimeOfEnter.Service;

public class AccountService(UserManager<AppUser> userManager, IOptions<JWT> jwt,
    RoleManager<IdentityRole> roleManager) : IAccountService
{
    private readonly JWT jwt = jwt.Value;

    public async Task<TokenDto> RegisterAsync(RegisterDto registerDto)
    {
        var TokenDto = new TokenDto();

        if (await userManager.FindByEmailAsync(registerDto.Email) is not null)

            return new TokenDto
            {
                IsAuthenticated = false,
                Massage = "Email is already registered!"
            };

        if (await userManager.FindByNameAsync(registerDto.UserName) is not null)
            return new TokenDto
            {
                IsAuthenticated = false,
                Massage = "Username is already registered!"
            };

        var appUser = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email
        };

        var result = await userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
            TokenDto.IsAuthenticated = false;
            TokenDto.Massage = "Password is Week Must Contain Upper and numbers ";
            return TokenDto;
        }

        var jwtSecurity = await CreateJwtToken(appUser);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        var validTo = jwtSecurity.ValidTo;

        var refreshToken = GetRefreshToken();
        appUser.RefreshTokens ??= [];
        appUser.RefreshTokens.Add(refreshToken);
        await userManager.UpdateAsync(appUser);

        return new TokenDto(
            Massage: "Register Succucessfully",
            IsAuthenticated: true,
            AccessToken: tokenString,
            AccessTokenExpiresOn: validTo,
            RefreshToken: refreshToken.Token,
            RefreshTokenExpiresOn: refreshToken.ExpireON
        );
    }


    public async Task<TokenDto> LoginAsync(LoginDto loginDto)
    {
        AppUser appUser = await userManager.FindByEmailAsync(loginDto.Email);
        if (appUser == null) return null;

        bool found = await userManager.CheckPasswordAsync(appUser, loginDto.Password);
        if (!found) return null;

        var jwtSecurity = await CreateJwtToken(appUser);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        var validTo = jwtSecurity.ValidTo;

        if (appUser.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = appUser.RefreshTokens.FirstOrDefault(a => a.IsActive);

            return new TokenDto
            {
                Massage = "Login Succucessfully",
                IsAuthenticated = true,
                AccessToken = tokenString,
                AccessTokenExpiresOn = validTo,
                RefreshToken = activeRefreshToken.Token,
                RefreshTokenExpiresOn = activeRefreshToken.ExpireON
            };
        }
        else
        {
            var refreshToken = GetRefreshToken();
            appUser.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(appUser);

            return new TokenDto
            {
                Massage = "Login Succucessfully",
                IsAuthenticated = true,
                AccessToken = tokenString,
                AccessTokenExpiresOn = validTo,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresOn = refreshToken.ExpireON
            };
        }
    }

    public async Task<string> AddRoleAsync(AddRole addRole)
    {
        var user = await userManager.FindByIdAsync(addRole.UserId);

        if (user == null || !await roleManager.RoleExistsAsync(addRole.Role))
            return "Invalid user ID or Role";

        if (await userManager.IsInRoleAsync(user, addRole.Role))
            return "User already assigned to this role";

        var result = await userManager.AddToRoleAsync(user, addRole.Role);

        if (!result.Succeeded)
            return string.Empty;

        return "Role added successfully";

    }
    public async Task<TokenDto> RefreshTokenAsync(string token)
    {
        var TokenDto = new TokenDto();

        var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
        {
            TokenDto.Massage = "Invalid token";
            return TokenDto;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            TokenDto.Massage = "InActive token";
            return TokenDto;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = GetRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);
        var accsessToken = await CreateJwtToken(user);

        TokenDto.IsAuthenticated = true;
        TokenDto.AccessToken = new JwtSecurityTokenHandler().WriteToken(accsessToken);

        return (TokenDto);

    }

    private static RefreshToken GetRefreshToken()
    {
        var randomNumber = new byte[32];

        using var generator = RandomNumberGenerator.Create();

        generator.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpireON = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow
        };
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            return false;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return false;

        refreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        return true;

    }

    private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
    {

        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Add(new Claim(ClaimTypes.Email, user.Email));

        var userRole = await userManager.GetRolesAsync(user);

        foreach (var item in userRole)
        {
            claims.Add(new Claim(ClaimTypes.Role, item));
        }

        var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecritKey));

        var signingCred = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtSecurity = new JwtSecurityToken(
            audience: jwt.AudienceIP,
            issuer: jwt.IssuerIP,
            expires: DateTime.UtcNow.AddMinutes(jwt.Expiration),
            claims: claims,
            signingCredentials: signingCred
            );
        return jwtSecurity;
    }

}



