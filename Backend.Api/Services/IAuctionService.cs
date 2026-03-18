using Backend.Api.Entities;
using Backend.Api.DTOs;

namespace Backend.Api.Services;

public interface IAuctionService
{
    Task<IEnumerable<AuctionResponse>> GetActiveAuctionsAsync();
    Task<Auction?> GetByIdAsync(int id);
    Task<bool> PlaceBidAsync(BidRequest bidRequest);
    Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int id);
    Task<AuctionResponse> CreateAuctionAsync(CreateAuctionRequest request);

}