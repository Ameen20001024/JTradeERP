using MediatR;
namespace JTradeERP.Application.Features.MasterData.Suppliers.CreateSupplier;
public sealed record CreateSupplierCommand(string SupplierCode,string Name,string? Address,string? ContactInformation,string? PaymentTerms,string? TRN):IRequest<CreateSupplierResponse>;
