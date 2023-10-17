using Dapper;
using Lea.Data.StoredProcedures;
using System.Data;

namespace Lea.Data.Repositories;

/// <summary>
/// Base respositry class to provide common methods for execuing database commands
/// </summary>
public abstract class Repository : IRepository
{
    public Repository(IDbConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    protected IDbConnectionFactory ConnectionFactory { get; }

    protected async Task<int> ExecuteAsync(IDatabaseCommand databaseCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(databaseCommand);

        var command = databaseCommand.GetCommand(cancellationToken);

        using var connection = await ConnectionFactory.OpenDbConnectionAsync(cancellationToken).ConfigureAwait(false);

        int rows = await connection.ExecuteAsync(command).ConfigureAwait(false);
        return rows;
    }

    protected async Task<T?> QuerySingleOrDefaultAsync<T>(IDatabaseCommand databaseCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(databaseCommand);

        var command = databaseCommand.GetCommand(cancellationToken);

        using var connection = await ConnectionFactory.OpenDbConnectionAsync(cancellationToken).ConfigureAwait(false);

        T? value = await connection.QuerySingleOrDefaultAsync<T>(command);
        return value;
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(IDatabaseCommand databaseCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(databaseCommand);

        var command = databaseCommand.GetCommand(cancellationToken);

        using var connection = await ConnectionFactory.OpenDbConnectionAsync(cancellationToken).ConfigureAwait(false);

        var items = await connection.QueryAsync<T>(command).ConfigureAwait(false);

        return items;
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(IDatabaseCommand databaseCommand, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(databaseCommand);

        if (transaction.Connection is null)
        {
            throw new InvalidOperationException("No connection associated with database transaction");
        }

        var command = databaseCommand.GetCommand(transaction, cancellationToken);

        var items = await transaction.Connection.QueryAsync<T>(command).ConfigureAwait(false);

        return items;
    }
}
