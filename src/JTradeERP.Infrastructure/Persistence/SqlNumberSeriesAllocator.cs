using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using JTradeERP.Application.Common.Billing;

namespace JTradeERP.Infrastructure.Persistence;

public sealed class SqlNumberSeriesAllocator(JTradeERPDbContext db) : INumberSeriesAllocator
{
    public async Task<string> AllocateAsync(string documentType, CancellationToken cancellationToken)
    {
        var connection = (SqlConnection)db.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction;
        command.CommandText = "UPDATE dbo.NumberSeries WITH (UPDLOCK, ROWLOCK) SET NextNumber = NextNumber + 1 OUTPUT INSERTED.Prefix, DELETED.NextNumber, INSERTED.Padding WHERE DocumentType = @documentType;";
        command.Parameters.Add(new SqlParameter("@documentType", SqlDbType.NVarChar, 50) { Value = documentType });
        var openedHere = connection.State != ConnectionState.Open;
        try
        {
            if (openedHere) await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken)) throw new InvalidOperationException($"Number series '{documentType}' is not configured.");
            var prefix = reader.GetString(0); var next = reader.GetInt64(1); var padding = reader.GetInt32(2);
            return prefix + next.ToString().PadLeft(padding, '0');
        }
        finally
        {
            if (openedHere) await connection.CloseAsync();
        }
    }
}
