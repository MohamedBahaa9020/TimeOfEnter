var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

RecurringJob.AddOrUpdate<ICleanNoneActiveDateService>(
    "clean-non-active-dates",
        service => service.DeleteNunActiveDate(),
        Cron.Minutely
        );
RecurringJob.AddOrUpdate<UpdateActivationOfDateService>(
    "update-activation-date",
        service => service.UpdateDate(),
        Cron.Minutely
        );

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
