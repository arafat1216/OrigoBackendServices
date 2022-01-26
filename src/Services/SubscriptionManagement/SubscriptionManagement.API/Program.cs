using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//https://andrewlock.net/exploring-dotnet-6-part-1-looking-inside-configurationmanager-in-dotnet-6/


builder.Services.AddControllers();

builder.Services.AddDbContext<SubscriptionManagmentContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SubscriptionManagmentConnectionString")
    ));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
