using Microsoft.AspNetCore.SignalR;
using Backend.Api.Hubs;
namespace Backend.Api.Services;

public class AuctionNotificationService : IAuctionNotificationService
{
    private readonly IHubContext<AuctionHub> _hubContext;

    public AuctionNotificationService(IHubContext<AuctionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewBidAsync(int auctionId, decimal newPrice, string bidderName)
    {
        await _hubContext.Clients.Group(auctionId.ToString())
            .SendAsync("ReceiveNewBid", new { auctionId, newPrice, bidderName });
    }

    public async Task NotifyAuctionClosedAsync(int auctionId, string winnerName)
    {
        await _hubContext.Clients.Group(auctionId.ToString())
            .SendAsync("AuctionClosed", new { auctionId, winnerName });
    }
}