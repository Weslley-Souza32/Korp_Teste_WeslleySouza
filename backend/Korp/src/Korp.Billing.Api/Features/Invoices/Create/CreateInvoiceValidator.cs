using FluentValidation;

namespace Korp.Billing.Api.Features.Invoices.Create
{
    public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
    {
        public CreateInvoiceValidator()
        {
            RuleFor(invoice => invoice.Items)
                .NotEmpty();

            RuleFor(invoice => invoice.Items)
                .Must(items =>
                    items.Select(item => item.ProductId)
                        .Distinct()
                        .Count() == items.Count)
                .WithMessage("The invoice cannot contain duplicate products.");

            RuleForEach(invoice => invoice.Items)
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
