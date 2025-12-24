using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeOfEnter.DTO;
using TimeOfEnter.Infrastructure.Helper;
using TimeOfEnter.Service.Interfaces;
using static TimeOfEnter.Common.Responses.RoleResponse;

namespace TimeOfEnter.Service;
public class AccountService(UserManager<AppUser> userManager, IOptions<JwtOptions> jwtOptions,
    RoleManager<IdentityRole> roleManager) : IAccountService
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;
    public async Task<TokenDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await userManager.FindByEmailAsync(registerDto.Email) is not null)

            return new TokenDto
            (
                IsAuthenticated:false,
                Massage:"Email is already registered!",
                AccessToken:"",
                AccessTokenExpiresOn:null,
                RefreshToken:"",
                RefreshTokenExpiresOn:null
            );

        if (await userManager.FindByNameAsync(registerDto.UserName) is not null)
            return new TokenDto
            (
                IsAuthenticated:false,
                Massage:"Username is already registered!",
                AccessToken: "",
                AccessTokenExpiresOn: null,
                RefreshToken: "",
                RefreshTokenExpiresOn: null
            );

        var appUser = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email
        };

        var result = await userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
            
            return new TokenDto(
                IsAuthenticated:false,
                Massage: "Password is Week must contain uppercase letters, numbers, and special characters.",
                AccessToken: "",
                AccessTokenExpiresOn: null,
                RefreshToken: "",
                RefreshTokenExpiresOn: null
                );

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
        AppUser? appUser = await userManager.FindByEmailAsync(loginDto.Email);
        if (appUser == null) return new TokenDto(
                IsAuthenticated: false,
                Massage: "Email or Password Invalid",
                AccessToken: "",
                AccessTokenExpiresOn: null,
                RefreshToken: "",
                RefreshTokenExpiresOn: null
                );

        bool found = await userManager.CheckPasswordAsync(appUser, loginDto.Password);
        if (!found)
            return new TokenDto(
                IsAuthenticated: false,
                Massage: "Email or Password Invalid",
                AccessToken: "",
                AccessTokenExpiresOn: null,
                RefreshToken: "",
                RefreshTokenExpiresOn: null
                );

        var jwtSecurity = await CreateJwtToken(appUser);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        var validTo = jwtSecurity.ValidTo;

        if (appUser.RefreshTokens != null && appUser.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = appUser.RefreshTokens.FirstOrDefault(a => a.IsActive);

            return new TokenDto
            (
                Massage:"Login Succucessfully",
                IsAuthenticated:true,
                AccessToken:tokenString,
                AccessTokenExpiresOn:validTo,
                RefreshToken:activeRefreshToken!.Token,
                RefreshTokenExpiresOn:activeRefreshToken.ExpireON
            );
        }
        else
        {
            var refreshToken = GetRefreshToken();
            appUser.RefreshTokens ??= [];
            appUser.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(appUser);

            return new TokenDto
            (
                Massage:"Login Succucessfully",
                IsAuthenticated:true,
                AccessToken:tokenString,
                AccessTokenExpiresOn:validTo,
                RefreshToken:refreshToken.Token,
                RefreshTokenExpiresOn:refreshToken.ExpireON
            );
        }
    }

    public async Task<RoleResult> AddRoleAsync(AddRole addRole)
    {
        var user = await userManager.FindByIdAsync(addRole.UserId);

        if (user == null || !await roleManager.RoleExistsAsync(addRole.Role))
            return new RoleResult(false, "Invalid user ID or Role");

        if (await userManager.IsInRoleAsync(user, addRole.Role))
            return new RoleResult(false, "User already assigned to this role");

        var result = await userManager.AddToRoleAsync(user, addRole.Role);

        return !result.Succeeded ? new RoleResult(false, "Failed to add role to user") : new RoleResult(true, "Role added successfully");
    }
    public async Task<TokenDto> RefreshTokenAsync(string token)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u!.RefreshTokens!.Any(t => t.Token == token));

        if (user == null || user.RefreshTokens is null)
        {
            return new TokenDto(
                IsAuthenticated:false,
                Massage: "Invalid token",
                AccessToken: "",
                AccessTokenExpiresOn: null,
                RefreshToken: "",
                RefreshTokenExpiresOn: null
                );
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            return new TokenDto(
                IsAuthenticated: false,
                Massage: "InActive token",
                AccessToken: "",
                AccessTokenExpiresOn: null,
                RefreshToken: "",
                RefreshTokenExpiresOn: null);
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = GetRefreshToken();
        user.RefreshTokens ??= [];
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);
        var accsessToken = await CreateJwtToken(user);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(accsessToken);
        var validTo = accsessToken.ValidTo;

        return new TokenDto(
       Massage: "Token Created Successfully",
       IsAuthenticated: true,
       AccessToken: tokenString,
       AccessTokenExpiresOn: validTo,
       RefreshToken: newRefreshToken.Token,
       RefreshTokenExpiresOn: newRefreshToken.ExpireON
       );

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
        var user = await userManager.Users.SingleOrDefaultAsync(u => u!.RefreshTokens!.Any(t => t.Token == token));

        if (user == null || user.RefreshTokens is null)
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

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user!.Email!),
        ];

        var userRole = await userManager.GetRolesAsync(user);

        foreach (var item in userRole)
        {
            claims.Add(new Claim(ClaimTypes.Role, item));
        }

        var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecritKey));

        var signingCred = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtSecurity = new(
            audience: jwtOptions.AudienceIP,
            issuer: jwtOptions.IssuerIP,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Expiration),
            claims: claims,
            signingCredentials: signingCred
            );
          return jwtSecurity;
    }

}



