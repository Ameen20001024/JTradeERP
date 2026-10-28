using MediatR;using JTradeERP.Application.Common.Persistence;
namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSuppliers;
public sealed class GetSuppliersHandler(IApplicationDbContext db):IRequestHandler<GetSuppliersQuery,IReadOnlyList<GetSuppliersResponse>>{public Task<IReadOnlyList<GetSuppliersResponse>> Handle(GetSuppliersQuery r,CancellationToken ct)=>Task.FromResult<IReadOnlyList<GetSuppliersResponse>>(db.Suppliers.OrderBy(x=>x.SupplierCode).Select(e=>new GetSuppliersResponse(e.Id,e.SupplierCode,e.Name,e.Address,e.ContactInformation,e.PaymentTerms,e.TRN)).ToList());}
