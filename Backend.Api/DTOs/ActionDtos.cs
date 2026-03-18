namespace Backend.Api.DTOs;

public record BidRequest(int AuctionId, string BidderName, decimal Amount);
public class AuctionResponse
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentHighestBid { get; set; }
    public string Status { get; set; } = string.Empty;
    public TimeSpan TimeRemaining { get; set; }
}

public record CreateAuctionRequest
(
 string ItemName ,
     string Description ,
     decimal StartingPrice ,
     DateTime EndTime 
);