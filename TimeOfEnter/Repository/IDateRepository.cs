using TimeOfEnter.Model;

namespace TimeOfEnter.Repository
{
    public interface IDateRepository
    {
        Task<List<Date>> GetAllasync();
        Task Addasync(Date RegisterData);
        Task DeleteRangeAsync(List<Date> dates);
        Task SaveAsync();
    }
}
