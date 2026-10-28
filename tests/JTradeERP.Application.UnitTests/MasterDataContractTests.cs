using JTradeERP.Application.Features.MasterData.Categories.CreateCategory;
using JTradeERP.Application.Features.MasterData.Categories.UpdateCategory;
using JTradeERP.Application.Features.MasterData.Customers.CreateCustomer;
using JTradeERP.Application.Features.MasterData.Customers.UpdateCustomer;
using JTradeERP.Application.Features.MasterData.Products.CreateProduct;
using JTradeERP.Application.Features.MasterData.Products.UpdateProduct;
using JTradeERP.Application.Features.MasterData.Suppliers.CreateSupplier;
using JTradeERP.Application.Features.MasterData.Suppliers.UpdateSupplier;
using JTradeERP.Application.Features.MasterData.TaxCodes.CreateTaxCode;
using JTradeERP.Application.Features.MasterData.TaxCodes.UpdateTaxCode;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.CreateUnitOfMeasure;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.UpdateUnitOfMeasure;
using JTradeERP.Application.Features.MasterData.Warehouses.CreateWarehouse;
using JTradeERP.Application.Features.MasterData.Warehouses.UpdateWarehouse;
using JTradeERP.Domain.MasterData;

namespace JTradeERP.Application.UnitTests;

public sealed class MasterDataContractTests
{
    [Fact]
    public void BodyBasedOperations_UseCommandsAsExplicitInputContracts()
    {
        Type[] commands = [typeof(CreateCategoryCommand), typeof(UpdateCategoryCommand), typeof(CreateUnitOfMeasureCommand), typeof(UpdateUnitOfMeasureCommand), typeof(CreateTaxCodeCommand), typeof(UpdateTaxCodeCommand), typeof(CreateProductCommand), typeof(UpdateProductCommand), typeof(CreateCustomerCommand), typeof(UpdateCustomerCommand), typeof(CreateSupplierCommand), typeof(UpdateSupplierCommand), typeof(CreateWarehouseCommand), typeof(UpdateWarehouseCommand)];
        Assert.All(commands, command => Assert.True(typeof(MediatR.IBaseRequest).IsAssignableFrom(command)));
        Assert.Equal(typeof(string), typeof(CreateProductCommand).GetProperty(nameof(CreateProductCommand.Name))!.PropertyType);
        Assert.Equal(typeof(decimal?), typeof(CreateCustomerCommand).GetProperty(nameof(CreateCustomerCommand.CreditLimit))!.PropertyType);
        Assert.DoesNotContain(typeof(CreateProductCommand).Assembly.GetTypes(), type => type.Namespace?.Contains("Features.MasterData") == true && type.Name.EndsWith("Request", StringComparison.Ordinal));
    }

    [Fact]
    public void ProductCommandsAndResponses_ExposeName_WithoutStockState()
    {
        Assert.NotNull(typeof(CreateProductCommand).GetProperty(nameof(CreateProductCommand.Name)));
        Assert.NotNull(typeof(UpdateProductCommand).GetProperty(nameof(UpdateProductCommand.Name)));
        var productResponseTypes = typeof(CreateProductResponse).Assembly.GetTypes().Where(x => x.Namespace?.Contains("Features.MasterData.Products") == true && x.Name.EndsWith("Response", StringComparison.Ordinal));
        Assert.All(productResponseTypes, type => { Assert.NotNull(type.GetProperty("Name")); Assert.DoesNotContain(type.GetProperties(), property => property.Name is "QuantityOnHand" or "QtyReserved" or "StockBalance"); });
    }

    [Fact]
    public void MasterDataResponseContracts_AreNotDomainEntities()
    {
        var domainEntities = new[] { typeof(Category), typeof(UnitOfMeasure), typeof(TaxCode), typeof(Product), typeof(Customer), typeof(Supplier), typeof(Warehouse) };
        var responses = typeof(CreateProductResponse).Assembly.GetTypes().Where(x => x.Namespace?.Contains("Features.MasterData") == true && x.Name.EndsWith("Response", StringComparison.Ordinal));
        Assert.All(responses, response => Assert.DoesNotContain(domainEntities, entity => entity == response || entity.IsAssignableFrom(response)));
    }

    [Fact]
    public void MasterDataHandlers_DeclareOnlyExplicitResponseContracts_ExceptNoContentDeletes()
    {
        var handlers = typeof(CreateProductCommand).Assembly.GetTypes().Where(type => type.Namespace?.Contains("Features.MasterData") == true && type.Name.EndsWith("Handler", StringComparison.Ordinal));
        foreach (var handler in handlers)
        {
            var contract = handler.GetInterfaces().Single(type => type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(MediatR.IRequestHandler<,>) || type.GetGenericTypeDefinition() == typeof(MediatR.IRequestHandler<>)));
            if (contract.GetGenericTypeDefinition() == typeof(MediatR.IRequestHandler<>))
            {
                Assert.Contains("Delete", handler.Namespace, StringComparison.Ordinal);
                continue;
            }

            var result = contract.GetGenericArguments()[1];
            var item = result.IsGenericType ? result.GetGenericArguments().Single() : result;
            Assert.EndsWith("Response", item.Name, StringComparison.Ordinal);
            Assert.False(item.Namespace?.StartsWith("JTradeERP.Domain", StringComparison.Ordinal) == true);
        }
    }
}
