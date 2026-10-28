using FluentValidation;
namespace JTradeERP.Application.Features.Billing.Quotations.ConvertQuotationToSalesOrder;

public sealed class ConvertQuotationToSalesOrderValidator:AbstractValidator<ConvertQuotationToSalesOrderCommand>
{
    public ConvertQuotationToSalesOrderValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.WarehouseId).NotEmpty();
    }
}
