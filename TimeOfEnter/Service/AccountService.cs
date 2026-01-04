using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
using TimeOfEnter.Errors;
using TimeOfEnter.Infrastructure.Options;
using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;
namespace TimeOfEnter.Service;

public class AccountService(UserManager<AppUser> userManager, IOptions<JwtOptions> jwtOptions,
    RoleManager<IdentityRole> roleManager, IAccountRepository accountRepository, IBookingRepository bookingRepository) : IAccountService
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;
    public async Task<ErrorOr<TokenResponse>> RegisterAsync(RegisterDto registerDto)
    {
        if (await userManager.FindByEmailAsync(registerDto.Email) is not null)
            return AccountErrors.EmailAlreadyInUse;

        if (await userManager.FindByNameAsync(registerDto.UserName) is not null)
            return AccountErrors.UsernameAlreadyInUse;

        var appUser = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email
        };

        var result = await userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
            return AccountErrors.WeakPassword;

        await userManager.AddToRoleAsync(appUser, "User");
        var jwtSecurity = await CreateJwtToken(appUser);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        var validTo = jwtSecurity.ValidTo;

        var refreshToken = GetRefreshToken();
        appUser.RefreshTokens ??= [];
        appUser.RefreshTokens.Add(refreshToken);
        await userManager.UpdateAsync(appUser);

        return new TokenResponse(
            Massage: "Register Succucessfully",
            IsAuthenticated: true,
            AccessToken: tokenString,
            AccessTokenExpiresOn: validTo,
            RefreshToken: refreshToken.Token,
            RefreshTokenExpiresOn: refreshToken.ExpireON
        );
    }
    public async Task<ErrorOr<TokenResponse>> LoginAsync(LoginDto loginDto)
    {
        AppUser? appUser = await userManager.FindByEmailAsync(loginDto.Email);
        if (appUser == null)
            return AccountErrors.InvalidEmailorPassword;

        bool found = await userManager.CheckPasswordAsync(appUser, loginDto.Password);
        if (!found)
            return AccountErrors.InvalidEmailorPassword;

        var jwtSecurity = await CreateJwtToken(appUser);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        var validTo = jwtSecurity.ValidTo;

        if (appUser.RefreshTokens != null && appUser.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = appUser.RefreshTokens.FirstOrDefault(a => a.IsActive);

            return new TokenResponse
            (
                Massage: "Login Succucessfully",
                IsAuthenticated: true,
                AccessToken: tokenString,
                AccessTokenExpiresOn: validTo,
                RefreshToken: activeRefreshToken!.Token,
                RefreshTokenExpiresOn: activeRefreshToken.ExpireON
            );
        }
        else
        {
            var refreshToken = GetRefreshToken();
            appUser.RefreshTokens ??= [];
            appUser.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(appUser);

            return new TokenResponse
            (
                Massage: "Login Succucessfully",
                IsAuthenticated: true,
                AccessToken: tokenString,
                AccessTokenExpiresOn: validTo,
                RefreshToken: refreshToken.Token,
                RefreshTokenExpiresOn: refreshToken.ExpireON
            );
        }
    }
    public async Task<ErrorOr<List<UserDataDto>>> AllUsersAsync()
    {
        var users = await accountRepository.GetAllUsersAsync();
        if (users == null || users.Count == 0)
        {
            return AccountErrors.NoUsersFound;
        }
        var userDtos = users.Select(user => new UserDataDto
        (
           UserName: user.UserName!,
           Email: user.Email!
        )).ToList();

        return userDtos;
    }
    public async Task<ErrorOr<UserDataDto>> GetProfileData(string userId)
    {
        var user = await accountRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return AccountErrors.UserNotFound;
        }
        var userDto = new UserDataDto
        (
           UserName: user.UserName!,
           Email: user.Email!
        );
        return userDto;
    }
    public async Task<ErrorOr<MessageResponse>> UpdateProfileData(string userId, UpdateDataDto updateData)
    {
        var user = await accountRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return AccountErrors.UserNotFound;
        }
        var result = await userManager.SetEmailAsync(user, updateData.Email);
        if (!result.Succeeded)
        {
            return AccountErrors.FailedUpdateEmail;
        }
        if (updateData.Photo != null && updateData.Photo.Length > 0)
        {
            var Extention = Path.GetExtension(updateData.Photo.FileName);
            var fileName = $"{Guid.NewGuid()}{Extention}";
            string uploadsFolder = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await updateData.Photo.CopyToAsync(fileStream);
            user.AttachmentPath = $"/uploads/{fileName}";
            await userManager.UpdateAsync(user);
        }
        return new MessageResponse("Email Updated Successfully.");
    }
    public async Task<ErrorOr<MessageResponse>> DeleteProfileData(string userId)
    {
        var user = await accountRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return AccountErrors.UserNotFound;
        }
        var bookings = await bookingRepository.GetAllBookingsAsync(userId);

        if (bookings.Any(b => !b.IsDeleted))
        {
            return AccountErrors.UserHasActiveBookings;
        }
        user.IsDeleted = true;
        await userManager.UpdateAsync(user);

        return new MessageResponse("User deleted successfully.");

    }
    public async Task<ErrorOr<Success>> AddRoleAsync(AddRoleDto addRole)
    {
        var user = await userManager.FindByIdAsync(addRole.UserId);

        var roleName = addRole.Role.Trim();

        if (user == null || !await roleManager.RoleExistsAsync(roleName))
            return AccountErrors.RoleNotFound;

        if (await userManager.IsInRoleAsync(user, roleName))
            return AccountErrors.CannotAssignRole;

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
            return AccountErrors.FailedAddRole;

        return Result.Success;
    }
    public async Task<ErrorOr<TokenResponse>> RefreshTokenAsync(string token)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u!.RefreshTokens!.Any(t => t.Token == token));

        if (user == null || user.RefreshTokens is null)
        {
            return AccountErrors.InvalidToken;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            return AccountErrors.ExpiredToken;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = GetRefreshToken();
        user.RefreshTokens ??= [];
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);
        var accsessToken = await CreateJwtToken(user);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(accsessToken);
        var validTo = accsessToken.ValidTo;

        return new TokenResponse(
           Massage: "Token Created Successfully",
           IsAuthenticated: true,
           AccessToken: tokenString,
           AccessTokenExpiresOn: validTo,
           RefreshToken: newRefreshToken.Token,
           RefreshTokenExpiresOn: newRefreshToken.ExpireON);
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
    public async Task<ErrorOr<Success>> RevokeTokenAsync(string token)
    {
        if (token == null)
            return AccountErrors.TokenRequired;
        var user = await userManager.Users.SingleOrDefaultAsync(u => u!.RefreshTokens!.Any(t => t.Token == token));

        if (user == null || user.RefreshTokens is null)
            return AccountErrors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return AccountErrors.ExpiredToken;

        refreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        return Result.Success;
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



