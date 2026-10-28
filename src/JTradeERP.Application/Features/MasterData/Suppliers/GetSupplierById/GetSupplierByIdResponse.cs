namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSupplierById;
public sealed record GetSupplierByIdResponse(Guid Id,string SupplierCode,string Name,string? Address,string? ContactInformation,string? PaymentTerms,string? TRN);
