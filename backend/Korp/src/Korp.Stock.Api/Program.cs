using FluentValidation;
using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Features.Products.Create;
using Korp.Stock.Api.Features.Products.GetAll;
using Korp.Stock.Api.Features.Products.GetById;
using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<StockDbContext>(options =>
   options.UseNpgsql(
       builder.Configuration.GetConnectionString("StockDatabase")));

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

builder.Services.AddScoped<CreateProductHandler>();

builder.Services.AddScoped<GetAllProductsHandler>();

builder.Services.AddScoped<GetProductByIdHandler>();

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
