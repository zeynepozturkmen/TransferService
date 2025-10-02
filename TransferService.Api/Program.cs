using Microsoft.EntityFrameworkCore;
using TransferService.Application.Services;
using TransferService.Domain.Interfaces;
using TransferService.Infrastructure.Context;
using TransferService.Infrastructure.ExternalServices;
using TransferService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<TransferDbContext>(opt => opt.UseInMemoryDatabase("TransferDb"));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransferService, TransferServiceApp>();
builder.Services.AddHttpClient<IFraudDetectionService, FraudDetectionService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();


builder.Services.AddControllers();
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
