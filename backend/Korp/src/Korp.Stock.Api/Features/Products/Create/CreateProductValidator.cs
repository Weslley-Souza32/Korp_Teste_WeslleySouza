using FluentValidation;

namespace Korp.Stock.Api.Features.Products.Create
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(product => product.Code)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(product => product.Description)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(product => product.StockQuantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}
