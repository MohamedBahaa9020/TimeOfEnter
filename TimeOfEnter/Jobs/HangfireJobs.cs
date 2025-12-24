using TimeOfEnter.Service.Interfaces;
namespace TimeOfEnter.Jobs
{
    public class HangfireJobs
    {
        public static void RegisterJobs(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<ICleanNoneActiveDateService>(
            "clean-non-active-dates",
            service => service.DeleteNoneActiveDate(),
            Cron.Daily
        );
            recurringJobManager.AddOrUpdate<IUpdateActivationOfDateService>
            (
            "update-activation-date",
            service => service.UpdateDate(),
            Cron.Minutely
            );
        }
    }
}
