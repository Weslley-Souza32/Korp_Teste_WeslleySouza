using FluentValidation;
using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Features.Invoices.Create;
using Korp.Billing.Api.Features.Invoices.GetAll;
using Korp.Billing.Api.Features.Invoices.GetById;
using Korp.Billing.Api.Infrastructure.Clients.Stock;
using Korp.Billing.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<BillingDbContext>(option =>
    option.UseNpgsql(
        builder.Configuration.GetConnectionString("BillingDatabase")));

builder.Services.AddHttpClient<IStockServiceClient, StockServiceClient>(httpClient =>
{
    var baseUrl = builder.Configuration["Services:Stock:BaseUrl"]
        ?? throw new InvalidOperationException("Stock service base URL is not configured.");

    httpClient.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateInvoiceValidator>();

builder.Services.AddScoped<CreateInvoiceHandler>();
builder.Services.AddScoped<GetAllInvoicesHandler>();
builder.Services.AddScoped<GetInvoiceByIdHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
