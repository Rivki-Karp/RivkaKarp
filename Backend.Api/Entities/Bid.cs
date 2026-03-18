using System.ComponentModel.DataAnnotations;
namespace Backend.Api.Entities;

public class Bid
{
    public int Id { get; set; }

    public int AuctionId { get; set; }
    public Auction? Auction { get; set; }

    [Required]
    public string BidderName { get; set; } = string.Empty; // במערכת אמיתית זה יהיה UserId

    public decimal Amount { get; set; }

    public DateTime BidTime { get; set; } = DateTime.UtcNow;
}