using Microsoft.EntityFrameworkCore;
using TradeTrack.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var connectionString =
    builder.Configuration.GetConnectionString("TradeTrack");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The TradeTrack database connection string is missing.");
}

builder.Services.AddDbContext<TradeTrackDbContext>(options =>
    options.UseMySQL(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();