using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Lea.Data;

public class OracleDbConnectionFactory : IDbConnectionFactory
{
    private readonly Func<OracleConnection> _connectionFactory;

    public OracleDbConnectionFactory(Func<OracleConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IDbConnection> OpenDbConnectionAsync(CancellationToken cancellationToken)
    {
        OracleConnection connection = _connectionFactory();
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    public IDbConnection CreateDbConnection()
    {
        return _connectionFactory();
    }
}
