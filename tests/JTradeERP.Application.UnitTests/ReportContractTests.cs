using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Features.Reports.SalesSummary;
using JTradeERP.Application.Features.Reports.StockMovement;
using JTradeERP.Application.Features.Reports.CustomerAging;

namespace JTradeERP.Application.UnitTests;

public sealed class ReportContractTests
{
    [Fact]
    public void SalesSummaryValidator_RejectsInvalidDateRangePagingAndSort()
    {
        var validator = new GetSalesSummaryValidator();
        var result = validator.Validate(new GetSalesSummaryQuery(
            FromDate: new DateOnly(2026, 2, 1), ToDate: new DateOnly(2026, 1, 1),
            Page: 0, PageSize: 501, SortBy: "GrandTotal; DROP TABLE Invoices", SortDirection: "asc;--"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(GetSalesSummaryQuery.ToDate));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(GetSalesSummaryQuery.SortBy));
    }

    [Fact]
    public void StockMovementValidator_RejectsInvalidDateRangeAndSort()
    {
        var result = new GetStockMovementValidator().Validate(new GetStockMovementQuery(
            FromUtc: DateTimeOffset.UtcNow, ToUtc: DateTimeOffset.UtcNow.AddDays(-1),
            SortBy: "PostedAt DESC;--", SourceDocumentType: new string('x', 65)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(GetStockMovementQuery.ToUtc));
    }

    [Fact]
    public void CustomerAgingValidator_RequiresAsOfDateAndAllowsOnlySafePagingAndSort()
    {
        var validator = new GetCustomerAgingValidator();
        Assert.False(validator.Validate(new GetCustomerAgingQuery(default)).IsValid);
        var invalid = validator.Validate(new GetCustomerAgingQuery(new DateOnly(2026, 6, 30), Page: 0,
            PageSize: 501, SortBy: "TotalOutstanding; DROP TABLE Invoices"));
        Assert.False(invalid.IsValid);
        Assert.Contains(invalid.Errors, x => x.PropertyName == nameof(GetCustomerAgingQuery.Page));
        Assert.Contains(invalid.Errors, x => x.PropertyName == nameof(GetCustomerAgingQuery.SortBy));
        Assert.True(validator.Validate(new GetCustomerAgingQuery(new DateOnly(2026, 6, 30))).IsValid);
    }

    [Theory]
    [InlineData("=HYPERLINK(\"https://bad.invalid\")", "'=HYPERLINK(")]
    [InlineData("normal, comma", "\"normal, comma\"")]
    [InlineData("quote \" here", "quote \"\" here")]
    public void CsvReportFormatter_QuotesCellsAndNeutralizesFormulaLikeText(string input, string expectedFragment)
    {
        var bytes = CsvReportFormatter.Create(["Value"], [input], x => [x]);
        var csv = System.Text.Encoding.UTF8.GetString(bytes);

        Assert.Contains(expectedFragment, csv);
    }
}
