using System.Diagnostics;

namespace JTradeERP.Application.Common.Inventory;

public static class InventoryPostingMetadata
{
    public static string CorrelationId => Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
}
