using TimeOfEnter;
using TimeOfEnter.Infrastructure.Middleware;
using TimeOfEnter.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
HangfireJobs.RegisterJobs(recurringJobManager);
app.UseHangfireDashboard("/hangfire");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);
