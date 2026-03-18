namespace Backend.Api.Services;

public interface IAuctionNotificationService
{
    Task NotifyNewBidAsync(int auctionId, decimal newPrice, string bidderName);
    Task NotifyAuctionClosedAsync(int auctionId, string winnerName);
}
