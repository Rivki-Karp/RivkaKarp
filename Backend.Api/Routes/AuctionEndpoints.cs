using Backend.Api.Services;
using Backend.Api.DTOs;
using Backend.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Routes;

public static class AuctionEndpoints
{
    public static void MapAuctionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auctions").WithTags("Auctions");

        // 1. קבלת כל המכירות הפעילות (מחזיר AuctionResponse)
        group.MapGet("/", async (IAuctionService auctionService) =>
        {
            var auctions = await auctionService.GetActiveAuctionsAsync();
            return Results.Ok(auctions);
        });

        // 2. קבלת מכירה ספציפית לפי ID
        group.MapGet("/{id}", async (int id, IAuctionService auctionService) =>
        {
            var auction = await auctionService.GetByIdAsync(id);
            return auction is not null ? Results.Ok(auction) : Results.NotFound();
        });

        // 3. יצירת מכירה חדשה - משתמש ב-CreateAuctionRequest כדי להסתיר שדות פנימיים
        group.MapPost("/", async ([FromBody] CreateAuctionRequest request, IAuctionService auctionService) =>
        {
            // וולידציה בסיסית ב-Endpoint
            if (request.StartingPrice <= 0)
            {
                return Results.BadRequest("Starting price must be greater than zero.");
            }

            if (request.EndTime <= DateTime.UtcNow)
            {
                return Results.BadRequest("End time must be in the future.");
            }

            var response = await auctionService.CreateAuctionAsync(request);

            // מחזיר 201 Created עם נתיב למשאב החדש והאובייקט הנקי
            return Results.Created($"/api/auctions/{response.Id}", response);
        })
        .WithName("CreateAuction")
        .WithOpenApi(); // מבטיח עדכון ויזואלי ב-Swagger

        // 4. הגשת הצעת מחיר
        group.MapPost("/place-bid", async ([FromBody] BidRequest bidRequest, IAuctionService auctionService) =>
        {
            var result = await auctionService.PlaceBidAsync(bidRequest);
            return result
                ? Results.Ok(new { message = "Bid placed successfully" })
                : Results.BadRequest(new { message = "Bid failed: Check amount or auction status" });
        });

        // 5. היסטוריית הצעות למכירה מסוימת
        group.MapGet("/{id}/bids", async (int id, IAuctionService auctionService) =>
        {
            var bids = await auctionService.GetBidsByAuctionIdAsync(id);
            return Results.Ok(bids);
        });
    }
}