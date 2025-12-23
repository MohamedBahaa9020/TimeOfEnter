using System;
using TimeOfEnter.Model;
using TimeOfEnter.Repository;

namespace TimeOfEnter.Service
{
    public class CleanNunActiveDateService(IDateRepository dateRepository)
    {
       

        public async Task DeleteNunActiveDate ()
        {
            var NunActiveDates =await dateRepository.GetAllasync();

            var DateIsFlase = NunActiveDates.Where(d=>d.IsActive == false).ToList();

           await dateRepository.DeleteRangeAsync(DateIsFlase);
        }

    }
}
