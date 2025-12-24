using TimeOfEnter.Repository;

namespace TimeOfEnter.Service;

public interface ICleanNoneActiveDateService
{
    Task DeleteNunActiveDate();
}

public class CleanNoneActiveDateService(IDateRepository dateRepository) : ICleanNoneActiveDateService
{
    public async Task DeleteNunActiveDate()
    {
        var dates = await dateRepository.GetAllasync();

        var noneActiveDates = dates.Where(d => d.IsActive == false).ToList();

        await dateRepository.DeleteRangeAsync(noneActiveDates);
    }
}

public class CleanNoneActiveDateJob : ICleanNoneActiveDateService
{
    public Task DeleteNunActiveDate()
    {
        throw new NotImplementedException();
    }
}
