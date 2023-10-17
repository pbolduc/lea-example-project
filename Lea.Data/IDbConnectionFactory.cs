using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Lea.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateDbConnection();

    Task<IDbConnection> OpenDbConnectionAsync(CancellationToken cancellationToken);
}
