using Microsoft.EntityFrameworkCore;
using TasksManager.Data;
using TasksManager.Models;

public class PeriodicCleanupService : BackgroundService
{
    private const int DaysToDelete = 30;
    private readonly IServiceProvider _serviceProvider;

    public PeriodicCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var cutoffDate = DateOnly
                .FromDateTime(DateTime.Today)
                .AddDays(-DaysToDelete);

            await context.TasksDB
                .Where(t => t.Status == TaskModel.TaskStatus.Deleted
                         && t.DeletedDate != null
                         && t.DeletedDate <= cutoffDate)
                .ExecuteDeleteAsync(stoppingToken);

            await Task.Delay(
                TimeSpan.FromDays(1),
                stoppingToken);
        }
    }
}