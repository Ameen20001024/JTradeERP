namespace JTradeERP.Application.Features.MasterData.Suppliers.UpdateSupplier;
public sealed record UpdateSupplierResponse(Guid Id,string SupplierCode,string Name,string? Address,string? ContactInformation,string? PaymentTerms,string? TRN);
