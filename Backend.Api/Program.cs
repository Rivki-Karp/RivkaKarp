using Backend.Api.Extensions;
using Backend.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// הגדרות Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseCors("AngularPolicy");
app.UseHttpsRedirection();

// רישום ה-Hub (הכתובת אליה ה-Angular יתחבר)
app.MapHub<AuctionHub>("/hubs/auction");

app.MapAuctionEndpoints();

app.Run();