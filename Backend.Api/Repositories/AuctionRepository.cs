using Backend.Api.Data;
using Backend.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;

    public AuctionRepository(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Auction>> GetAllAsync()
    {
        return await _context.Auctions.ToListAsync();
    }

    public async Task<Auction?> GetByIdAsync(int id)
    {
        return await _context.Auctions.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int id)
    {
        return await _context.Bids
            .Where(b => b.AuctionId == id)
            .OrderByDescending(b => b.BidTime)
            .ToListAsync();
    }

    // וודאי שזו הפעם היחידה שהפונקציה הזו מופיעה!
    public async Task<Auction> AddAsync(Auction auction)
    {
        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync();
        return auction;
    }

    public async Task<bool> UpdateAuctionWithBidAsync(Auction auction, Bid bid)
    {
        try
        {
            _context.Bids.Add(bid);
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}