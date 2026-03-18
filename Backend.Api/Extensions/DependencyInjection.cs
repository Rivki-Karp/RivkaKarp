using Backend.Api.Data;
using Backend.Api.Repositories;
using Backend.Api.Services;
using Backend.Api.BackgroundServices; // ודאי שזה שם התיקייה החדשה שלך
using Backend.Api.Middleware;       // עבור ה-GlobalExceptionHandler
using Backend.Api.Hubs;             // עבור ה-SignalR Hub
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuctionDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddScoped<IAuctionRepository, AuctionRepository>();

        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<IAuctionNotificationService, AuctionNotificationService>();

        services.AddHostedService<AuctionClosingWorker>();

        services.AddSignalR();

        services.AddCors(options =>
        {
            options.AddPolicy("AngularPolicy", policyBuilder =>
            {
                policyBuilder.WithOrigins("http://localhost:4200")
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                             .AllowCredentials(); // חובה כדי ש-SignalR יעבוד עם Cookies/Auth
            });
        });

        return services;
    }
}