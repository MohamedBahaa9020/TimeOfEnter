using TimeOfEnter.DTO;
using static TimeOfEnter.Common.Responses.RoleResponse;

namespace TimeOfEnter.Service.Interfaces;

public interface IAccountService
{
    Task<TokenDto> RegisterAsync(RegisterDto registerDto);
    Task<TokenDto> LoginAsync(LoginDto loginDto);
    Task<RoleResult> AddRoleAsync(AddRole addRole);
    Task<TokenDto> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
}
