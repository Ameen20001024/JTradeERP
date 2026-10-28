namespace JTradeERP.Application.Common.Inventory;

/// <summary>Guards the DECIMAL(18,4) inventory columns against overflow and implicit scale rounding.</summary>
public static class InventoryPrecision
{
    public const decimal Maximum = 99_999_999_999_999.9999m;

    public static bool IsQuantity(decimal value) => Math.Abs(value) <= Maximum && decimal.Round(value, 4) == value;
    public static bool IsUnitCost(decimal value) => value >= 0 && value <= Maximum && decimal.Round(value, 4) == value;
}
