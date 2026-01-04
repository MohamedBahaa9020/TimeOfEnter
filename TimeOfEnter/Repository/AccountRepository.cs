using Microsoft.EntityFrameworkCore;
using TimeOfEnter.Database;

namespace TimeOfEnter.Repository;

public class AccountRepository(DateContext context) : IAccountRepository
{
    public async Task<List<AppUser>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }
    public async Task<AppUser?> GetByIdAsync(string userId)
    {
        return await context.Users.FindAsync(userId);
    }
}
