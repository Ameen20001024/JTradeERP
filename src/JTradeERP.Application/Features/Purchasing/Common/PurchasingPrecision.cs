namespace JTradeERP.Application.Features.Purchasing.Common;

public static class PurchasingPrecision
{
    public static bool Quantity(decimal value) => Fits(value, 18, 4);
    public static bool UnitCost(decimal value) => Fits(value, 18, 4);
    public static bool Money(decimal value) => Fits(value, 18, 2);
    public static bool TaxRate(decimal value) => Fits(value, 9, 4) && value is >= 0 and <= 100;

    private static bool Fits(decimal value, int precision, int scale)
    {
        var maximumIntegerDigits = precision - scale;
        var absolute = Math.Abs(value);
        return decimal.Round(value, scale, MidpointRounding.ToZero) == value
            && absolute < (decimal)Math.Pow(10, maximumIntegerDigits);
    }
}
