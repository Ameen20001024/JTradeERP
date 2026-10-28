using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using JTradeERP.Application.Features.MasterData.Categories.CreateCategory;
using JTradeERP.Application.Features.MasterData.Categories.UpdateCategory;
using JTradeERP.Application.Features.MasterData.Categories.GetCategoryById;
using JTradeERP.Application.Features.MasterData.Categories.GetCategories;
using JTradeERP.Application.Features.MasterData.Categories.ActivateCategory;
using JTradeERP.Application.Features.MasterData.Categories.DeactivateCategory;
using JTradeERP.Application.Features.MasterData.Categories.DeleteCategory;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.CreateUnitOfMeasure;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.UpdateUnitOfMeasure;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.GetUnitOfMeasureById;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.GetUnitsOfMeasure;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.DeleteUnitOfMeasure;
using JTradeERP.Application.Features.MasterData.TaxCodes.CreateTaxCode;
using JTradeERP.Application.Features.MasterData.TaxCodes.UpdateTaxCode;
using JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodeById;
using JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodes;
using JTradeERP.Application.Features.MasterData.TaxCodes.ActivateTaxCode;
using JTradeERP.Application.Features.MasterData.TaxCodes.DeactivateTaxCode;
using JTradeERP.Application.Features.MasterData.Products.CreateProduct;
using JTradeERP.Application.Features.MasterData.Products.UpdateProduct;
using JTradeERP.Application.Features.MasterData.Products.GetProductById;
using JTradeERP.Application.Features.MasterData.Products.GetProducts;
using JTradeERP.Application.Features.MasterData.Products.ActivateProduct;
using JTradeERP.Application.Features.MasterData.Products.DeactivateProduct;
using JTradeERP.Application.Features.MasterData.Customers.CreateCustomer;
using JTradeERP.Application.Features.MasterData.Customers.UpdateCustomer;
using JTradeERP.Application.Features.MasterData.Customers.GetCustomerById;
using JTradeERP.Application.Features.MasterData.Customers.GetCustomers;
using JTradeERP.Application.Features.MasterData.Customers.DeleteCustomer;
using JTradeERP.Application.Features.MasterData.Suppliers.CreateSupplier;
using JTradeERP.Application.Features.MasterData.Suppliers.UpdateSupplier;
using JTradeERP.Application.Features.MasterData.Suppliers.GetSupplierById;
using JTradeERP.Application.Features.MasterData.Suppliers.GetSuppliers;
using JTradeERP.Application.Features.MasterData.Suppliers.DeleteSupplier;
using JTradeERP.Application.Features.MasterData.Warehouses.CreateWarehouse;
using JTradeERP.Application.Features.MasterData.Warehouses.UpdateWarehouse;
using JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouseById;
using JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouses;
using JTradeERP.Application.Features.MasterData.Warehouses.ActivateWarehouse;
using JTradeERP.Application.Features.MasterData.Warehouses.DeactivateWarehouse;

namespace JTradeERP.Application.Features.MasterData;
public static class MasterDataEndpointExtensions
{
    public static IEndpointRouteBuilder MapMasterDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/master-data").RequireAuthorization().AddEndpointFilter<MasterDataPermissionFilter>();
        CreateCategoryEndpoint.Map(group); UpdateCategoryEndpoint.Map(group);
        GetCategoryByIdEndpoint.Map(group); GetCategoriesEndpoint.Map(group); ActivateCategoryEndpoint.Map(group); DeactivateCategoryEndpoint.Map(group); DeleteCategoryEndpoint.Map(group);
        CreateUnitOfMeasureEndpoint.Map(group); UpdateUnitOfMeasureEndpoint.Map(group);
        GetUnitOfMeasureByIdEndpoint.Map(group); GetUnitsOfMeasureEndpoint.Map(group); DeleteUnitOfMeasureEndpoint.Map(group);
        CreateTaxCodeEndpoint.Map(group); UpdateTaxCodeEndpoint.Map(group); GetTaxCodeByIdEndpoint.Map(group); GetTaxCodesEndpoint.Map(group); ActivateTaxCodeEndpoint.Map(group); DeactivateTaxCodeEndpoint.Map(group);
        CreateProductEndpoint.Map(group); UpdateProductEndpoint.Map(group); GetProductByIdEndpoint.Map(group); GetProductsEndpoint.Map(group); ActivateProductEndpoint.Map(group); DeactivateProductEndpoint.Map(group);
        CreateCustomerEndpoint.Map(group); UpdateCustomerEndpoint.Map(group); GetCustomerByIdEndpoint.Map(group); GetCustomersEndpoint.Map(group); DeleteCustomerEndpoint.Map(group);
        CreateSupplierEndpoint.Map(group); UpdateSupplierEndpoint.Map(group); GetSupplierByIdEndpoint.Map(group); GetSuppliersEndpoint.Map(group); DeleteSupplierEndpoint.Map(group);
        CreateWarehouseEndpoint.Map(group); UpdateWarehouseEndpoint.Map(group); GetWarehouseByIdEndpoint.Map(group); GetWarehousesEndpoint.Map(group); ActivateWarehouseEndpoint.Map(group); DeactivateWarehouseEndpoint.Map(group);
        return endpoints;
    }
}
