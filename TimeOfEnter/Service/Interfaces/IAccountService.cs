using TimeOfEnter.DTO;

namespace TimeOfEnter.Service;

public interface IAccountService
{
    Task<TokenDto> RegisterAsync(RegisterDto registerDto);
    Task<TokenDto> LoginAsync(LoginDto loginDto);
    Task<string> AddRoleAsync(AddRole addRole);
    Task<TokenDto> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
}
