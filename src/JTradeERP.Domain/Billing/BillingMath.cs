namespace JTradeERP.Domain.Billing;

internal static class BillingMath
{
    public static decimal Money(decimal amount) => decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
}
