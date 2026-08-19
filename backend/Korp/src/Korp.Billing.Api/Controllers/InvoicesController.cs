using FluentValidation;
using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Features.Invoices.Create;
using Korp.Billing.Api.Features.Invoices.GetAll;
using Korp.Billing.Api.Features.Invoices.GetById;
using Korp.Billing.Api.Features.Invoices.Print;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Billing.Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly CreateInvoiceHandler _createInvoiceHandler;
        private readonly GetAllInvoicesHandler _getAllInvoicesHandler;
        private readonly IValidator<CreateInvoiceRequest> _createInvoiceValidator;
        private readonly GetInvoiceByIdHandler _getInvoiceByIdHandler;
        private readonly PrintInvoiceHandler _printInvoiceHandler;

        public InvoicesController(CreateInvoiceHandler createInvoiceHandler, IValidator<CreateInvoiceRequest> createInvoiceValidator, GetAllInvoicesHandler getAllInvoicesHandler, GetInvoiceByIdHandler getInvoiceByIdHandler, PrintInvoiceHandler printInvoiceHandler)
        {
            _createInvoiceHandler = createInvoiceHandler;
            _createInvoiceValidator = createInvoiceValidator;
            _getAllInvoicesHandler = getAllInvoicesHandler;
            _getInvoiceByIdHandler = getInvoiceByIdHandler;
            _printInvoiceHandler = printInvoiceHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _createInvoiceValidator.ValidateAsync(
                request,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new RequestValidationException(
                    validationResult.Errors);
            }

            var response = await _createInvoiceHandler.HandleAsync(
                request,
                cancellationToken);

            return Created(
                $"/api/invoices/{response.Id}",
                response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _getAllInvoicesHandler.HandleAsync(
                cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        { 
            var response = await _getInvoiceByIdHandler.HandleAsync(
                id,
                cancellationToken);

            return Ok(response);
        }

        [HttpPost("{id:guid}/print")]
        public async Task<IActionResult> PrintAsync( Guid id, CancellationToken cancellationToken)
        {
            var response = await _printInvoiceHandler.HandleAsync(
                id,
                cancellationToken);

            return Ok(response);
        }
    }
}