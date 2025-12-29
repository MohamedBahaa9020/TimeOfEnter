using ErrorOr;
using TimeOfEnter.Common.Responses;
using TimeOfEnter.DTO;
namespace TimeOfEnter.Service.Interfaces;

public interface IAccountService
{
    Task<ErrorOr<TokenResponse>> RegisterAsync(RegisterDto registerDto);
    Task<ErrorOr<TokenResponse>> LoginAsync(LoginDto loginDto);
    Task<ErrorOr<Success>> AddRoleAsync(AddRoleDto addRole);
    Task<ErrorOr<TokenResponse>> RefreshTokenAsync(string token);
    Task<ErrorOr<Success>> RevokeTokenAsync(string token);
}
