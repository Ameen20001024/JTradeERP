namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSuppliers;
public sealed record GetSuppliersResponse(Guid Id,string SupplierCode,string Name,string? Address,string? ContactInformation,string? PaymentTerms,string? TRN);
