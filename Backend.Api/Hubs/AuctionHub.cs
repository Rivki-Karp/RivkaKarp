using Microsoft.AspNetCore.SignalR;

namespace Backend.Api.Hubs;

public class AuctionHub : Hub
{
    // בשלב זה ה-Hub יכול להיות ריק. 
    // הוא משמש כ"מזהה" עבור ה-SignalR כדי לדעת לאן לשלוח הודעות.
    public async Task JoinAuctionGroup(int auctionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, auctionId.ToString());
    }
}