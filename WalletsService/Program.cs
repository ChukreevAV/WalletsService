using Microsoft.EntityFrameworkCore;
using WalletsService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<WalletsContext>(opt
    => opt.UseInMemoryDatabase("Wallets"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();