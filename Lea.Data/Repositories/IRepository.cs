using Dapper;
using Lea.Data.StoredProcedures;
using System.Data;

namespace Lea.Data.Repositories;

/// <summary>
/// Marker interface
/// </summary>
public interface IRepository
{
}

public abstract class Repository : IRepository
{
    public Repository(IDbConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    protected IDbConnectionFactory ConnectionFactory { get; }

    protected async Task<int> ExecuteAsync(IStoredProcedure procedure, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(procedure);

        var command = procedure.GetCommand(cancellationToken);

        using var connection = await ConnectionFactory.OpenDbConnectionAsync(cancellationToken).ConfigureAwait(false);

        int rows = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return rows;
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(IStoredProcedure procedure, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(procedure);

        var command = procedure.GetCommand(cancellationToken);

        using var connection = await ConnectionFactory.OpenDbConnectionAsync(cancellationToken).ConfigureAwait(false);

        var items = await connection.QueryAsync<T>(command).ConfigureAwait(false);

        return items;
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(IStoredProcedure procedure, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(procedure);

        if (transaction.Connection is null)
        {
            throw new InvalidOperationException("No connection associated with database transaction");
        }

        var command = procedure.GetCommand(transaction, cancellationToken);

        var items = await transaction.Connection.QueryAsync<T>(command).ConfigureAwait(false);

        return items;
    }
}
