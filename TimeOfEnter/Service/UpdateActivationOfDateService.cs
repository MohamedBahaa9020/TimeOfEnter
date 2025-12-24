using TimeOfEnter.Repository;
using TimeOfEnter.Service.Interfaces;

namespace TimeOfEnter.Service;

public class UpdateActivationOfDateService(IDateRepository dateRepository):IUpdateActivationOfDateService
{
    public async Task UpdateDate()
    {
        var allDates = await dateRepository.GetAllasync();
        if (allDates.Any(d => d.EndTime <= DateTime.UtcNow))
        {
            allDates.ForEach(d => d.IsActive = false);

            await dateRepository.SaveAsync();
        }
    }

}
