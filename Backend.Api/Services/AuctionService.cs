using Backend.Api.Data;
using Backend.Api.Entities;
using Backend.Api.DTOs;
using Backend.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Services;

public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _repository;
    private readonly IAuctionNotificationService _notificationService;

    public AuctionService(IAuctionRepository repository, IAuctionNotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<AuctionResponse>> GetActiveAuctionsAsync()
    {
        var auctions = await _repository.GetAllAsync();

        return auctions.Select(a => new AuctionResponse
        {
            Id = a.Id,
            ItemName = a.ItemName,
            CurrentHighestBid = a.CurrentHighestBid,
            Status = a.Status.ToString(),
            TimeRemaining = a.EndTime > DateTime.UtcNow ? a.EndTime - DateTime.UtcNow : TimeSpan.Zero
        }).ToList();
    }

    public async Task<Auction?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> PlaceBidAsync(BidRequest bidRequest)
    {
        var auction = await _repository.GetByIdAsync(bidRequest.AuctionId);

        if (auction == null || auction.Status != AuctionStatus.Active || bidRequest.Amount <= auction.CurrentHighestBid)
        {
            return false;
        }

        auction.CurrentHighestBid = bidRequest.Amount;

        var bid = new Bid
        {
            AuctionId = bidRequest.AuctionId,
            BidderName = bidRequest.BidderName,
            Amount = bidRequest.Amount,
            BidTime = DateTime.UtcNow
        };

        var success = await _repository.UpdateAuctionWithBidAsync(auction, bid);

        if (success)
        {
            await _notificationService.NotifyNewBidAsync(auction.Id, bidRequest.Amount, bidRequest.BidderName);
        }

        return success;
    }

    public async Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int id)
    {
        return await _repository.GetBidsByAuctionIdAsync(id);
    }
    public async Task<AuctionResponse> CreateAuctionAsync(CreateAuctionRequest request)
    {
        // יצירת ה-Entity מתוך ה-DTO
        var auction = new Auction
        {
            ItemName = request.ItemName,
            Description = request.Description,
            StartingPrice = request.StartingPrice,
            CurrentHighestBid = request.StartingPrice, // מחיר התחלתי הוא ההצעה הגבוהה כרגע
            StartTime = DateTime.UtcNow,
            EndTime = request.EndTime,
            Status = AuctionStatus.Active
        };

        var createdAuction = await _repository.AddAsync(auction);

        // החזרת DTO לצד לקוח (כדי לא לחשוף שדות פנימיים)
        return new AuctionResponse
        {
            Id = createdAuction.Id,
            ItemName = createdAuction.ItemName,
            CurrentHighestBid = createdAuction.CurrentHighestBid,
            Status = createdAuction.Status.ToString(),
            TimeRemaining = createdAuction.EndTime - DateTime.UtcNow
        };
    }
} 