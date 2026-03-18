
using System.ComponentModel.DataAnnotations;
namespace Backend.Api.Entities;

public class Auction
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string ItemName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal StartingPrice { get; set; }

    public decimal CurrentHighestBid { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public AuctionStatus Status { get; set; } = AuctionStatus.Active;

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}

public enum AuctionStatus
{
    Scheduled,
    Active,
    Closed
}