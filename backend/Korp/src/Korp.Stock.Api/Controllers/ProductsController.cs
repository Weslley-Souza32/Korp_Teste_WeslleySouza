using FluentValidation;
using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Features.Products.Create;
using Korp.Stock.Api.Features.Products.GetAll;
using Korp.Stock.Api.Features.Products.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Stock.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly CreateProductHandler _createProductHandler;
        private readonly IValidator<CreateProductRequest> _createProductValidator;
        private readonly GetAllProductsHandler _getAllProductsHandler;
        private readonly GetProductByIdHandler _getProductByIdHandler;

        public ProductsController(CreateProductHandler createProductHandler, IValidator<CreateProductRequest> createProductValidator, GetAllProductsHandler getAllProductsHandler, GetProductByIdHandler getProductByIdHandler)
        {
            _createProductHandler = createProductHandler;
            _createProductValidator = createProductValidator;
            _getAllProductsHandler = getAllProductsHandler;
            _getProductByIdHandler = getProductByIdHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _createProductValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new RequestValidationException(validationResult.Errors);
            }

            var response = await _createProductHandler.HandleAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById), 
                new { id = response.Id }, 
                response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _getAllProductsHandler.HandleAsync(cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _getProductByIdHandler.HandleAsync(id, cancellationToken);
            return Ok(response);
        }

    }
}
