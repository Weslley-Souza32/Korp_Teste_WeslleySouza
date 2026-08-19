using FluentValidation;

namespace Korp.Stock.Api.Features.Stock.Debit
{
    public class DebitStockValidator : AbstractValidator<DebitStockRequest>
    {
        public DebitStockValidator()
        {
            RuleFor(request => request.InvoiceId)
                .NotEmpty();

            RuleFor(request => request.Items)
                .NotEmpty();

            RuleForEach(request => request.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ProductId)
                        .NotEmpty();

                    item.RuleFor(x => x.Quantity)
                        .GreaterThan(0);
                });
        }
    }
}