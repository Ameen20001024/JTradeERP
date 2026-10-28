namespace JTradeERP.Application.Features.MasterData.Suppliers.CreateSupplier;
public sealed record CreateSupplierResponse(Guid Id,string SupplierCode,string Name,string? Address,string? ContactInformation,string? PaymentTerms,string? TRN);
