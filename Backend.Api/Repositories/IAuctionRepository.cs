using Backend.Api.Entities;

namespace Backend.Api.Repositories;

public interface IAuctionRepository
{
    Task<IEnumerable<Auction>> GetAllAsync();
    Task<Auction?> GetByIdAsync(int id);
    Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int id);
  
    Task<Auction> AddAsync(Auction auction); 

    Task<bool> UpdateAuctionWithBidAsync(Auction auction, Bid bid);

    Task SaveChangesAsync();
}