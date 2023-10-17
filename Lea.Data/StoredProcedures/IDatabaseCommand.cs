using Dapper;
using System.Data;

namespace Lea.Data.StoredProcedures;

public interface IDatabaseCommand
{
    CommandDefinition GetCommand();
    CommandDefinition GetCommand(CancellationToken cancellationToken = default);
    CommandDefinition GetCommand(IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}
