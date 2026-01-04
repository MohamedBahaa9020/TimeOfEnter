using TimeOfEnter.Service.Interfaces;
namespace TimeOfEnter.Jobs
{
    public class HangfireJobs
    {
        public static void RegisterJobs(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<IDateService>(
            "clean-non-active-dates",
            service => service.DeleteNoneActiveDate(),
            Cron.Daily
        );
            recurringJobManager.AddOrUpdate<IDateService>
            (
            "update-activation-date",
            service => service.UpdateDateActivation(),
            Cron.Minutely
            );
            recurringJobManager.AddOrUpdate<IAccountService>(
            "cleanup-unused-images",
            x => x.DeleteUnusedImagesAsync(),
            Cron.Daily
            );
        }
    }
}
