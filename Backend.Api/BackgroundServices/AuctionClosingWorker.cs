using Backend.Api.Data; 
using Backend.Api.Entities;
using Microsoft.EntityFrameworkCore;
namespace Backend.Api.BackgroundServices;

public class AuctionClosingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuctionClosingWorker> _logger;

    public AuctionClosingWorker(IServiceProvider serviceProvider, ILogger<AuctionClosingWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Auction Closing Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<IAuctionNotificationService>();

                    // 1. חילוץ מכירות שזמנן עבר והן עדיין בסטטוס Active
                    var expiredAuctions = await context.Auctions
                        .Where(a => a.Status == AuctionStatus.Active && a.EndTime < DateTime.UtcNow)
                        .ToListAsync();

                    if (expiredAuctions.Any())
                    {
                        foreach (var auction in expiredAuctions)
                        {
                            auction.Status = AuctionStatus.Closed;
                            _logger.LogInformation($"Auction {auction.Id} ({auction.ItemName}) has expired and is now closed.");

                            // 2. שליחת התראה בזמן אמת דרך SignalR (אבסטרקטי דרך ה-Service)
                            await notificationService.NotifyAuctionClosedAsync(auction.Id, "System");
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while closing auctions.");
            }

            // בדיקה פעם בדקה (60,000 מילי-שניות)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}