using JTradeERP.Domain.MasterData;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JTradeERP.Api.IntegrationTests;

public sealed class MasterDataModelTests
{
    [Fact]
    public void Model_MapsProductNameAsRequired_AndRestrictsMasterDataDeletes()
    {
        using var db = CreateContext(); var entity = db.Model.FindEntityType(typeof(Product))!;
        Assert.False(entity.FindProperty(nameof(Product.Name))!.IsNullable);
        Assert.Equal(200, entity.FindProperty(nameof(Product.Name))!.GetMaxLength());
        Assert.All(entity.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
    }

    [Fact]
    public void Model_MapsNullableCustomerCreditLimit_AndSeedsUaeVat5()
    {
        using var db = CreateContext(); var model = db.GetService<IDesignTimeModel>().Model; var customer = model.FindEntityType(typeof(Customer))!; var tax = model.FindEntityType(typeof(TaxCode))!;
        Assert.True(customer.FindProperty(nameof(Customer.CreditLimit))!.IsNullable);
        Assert.Contains(tax.GetSeedData(), x => (string)x["Code"]! == "VAT5" && (decimal)x["Rate"]! == 5m && (bool)x["IsActive"]!);
    }

    private static JTradeERPDbContext CreateContext() => new(new DbContextOptionsBuilder<JTradeERPDbContext>().UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=JTradeERP_ModelTest;Trusted_Connection=True;TrustServerCertificate=True").Options);
}
