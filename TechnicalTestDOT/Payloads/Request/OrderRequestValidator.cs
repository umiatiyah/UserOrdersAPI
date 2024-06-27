using FluentValidation;

namespace TechnicalTestDOT.Payloads.Request
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product Name is required.");
            RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Invoice Number is required.");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0);
            RuleFor(x => x.Username).NotEmpty().WithMessage("username is required.");
        }
    }
}
