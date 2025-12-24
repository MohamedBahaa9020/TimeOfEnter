using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;

namespace TimeOfEnter.Service;

public class CleanNoneActiveDateService(IDateRepository dateRepository) : ICleanNoneActiveDateService
{
    public async Task DeleteNoneActiveDate()
    {
        var dates = await dateRepository.GetAllasync();

        var noneActiveDates = dates.Where(d => d.IsActive == false).ToList();

        await dateRepository.DeleteRangeAsync(noneActiveDates);
    }
}

