﻿using Dapper;
using Dapper.Oracle;
using System.Data;

namespace Lea.Data.StoredProcedures;

/// <summary>
/// 
/// </summary>
public abstract class StoredProcedure : IDatabaseCommand
{
    private readonly Lazy<OracleDynamicParameters> _parameters;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected StoredProcedure(string sql)
    {
        Sql = sql ?? throw new ArgumentNullException(nameof(sql));
        _parameters = new Lazy<OracleDynamicParameters>(CreateParameters);
    }

    protected OracleDynamicParameters Parameters => _parameters.Value;

    protected string Sql { get; }

    protected virtual int? CommandTimeout { get; set; }
    protected virtual CommandFlags Flags { get; set; }


    public CommandDefinition GetCommand()
    {
        return new CommandDefinition(Sql, Parameters, null, CommandTimeout, CommandType.StoredProcedure, Flags, CancellationToken.None);
    }

    public CommandDefinition GetCommand(CancellationToken cancellationToken = default)
    {
        return new CommandDefinition(Sql, Parameters, null, CommandTimeout, CommandType.StoredProcedure, Flags, cancellationToken);
    }

    public CommandDefinition GetCommand(IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return new CommandDefinition(Sql, Parameters, transaction, CommandTimeout, CommandType.StoredProcedure, Flags, cancellationToken);
    }

    protected virtual void AddParameters(OracleDynamicParameters parameters)
    {
    }

    private OracleDynamicParameters CreateParameters()
    {
        var parameters = new OracleDynamicParameters();
        AddParameters(parameters);
        return parameters;
    }
}
