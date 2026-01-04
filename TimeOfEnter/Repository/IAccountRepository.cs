namespace TimeOfEnter.Repository
{
    public interface IAccountRepository
    {
        Task<List<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetByIdAsync(string userId);
        Task<List<string?>> SelectUsedImagesAsync();
    }
}
