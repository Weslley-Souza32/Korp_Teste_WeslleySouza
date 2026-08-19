using FluentValidation;
using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Features.Stock.Debit;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Stock.Api.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : ControllerBase
    {
        private readonly DebitStockHandler _debitStockHandler;
        private readonly IValidator<DebitStockRequest> _debitStockValidator;

        public StockController(DebitStockHandler debitStockHandler, IValidator<DebitStockRequest> debitStockValidator)
        {
            _debitStockHandler = debitStockHandler;
            _debitStockValidator = debitStockValidator;
        }

        [HttpPost("debit")]
        public async Task<IActionResult> DebitAsync(
            [FromBody] DebitStockRequest request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _debitStockValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new RequestValidationException(
                    validationResult.Errors);
            }

            var response = await _debitStockHandler.HandleAsync(
                request,
                cancellationToken);

            return Ok(response);
        }
    }
}