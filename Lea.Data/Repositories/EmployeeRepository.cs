using Dapper;
using Lea.Data.Models;
using Lea.Data.StoredProcedures;
using System.Data;
using System.Diagnostics;

namespace Lea.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Add an employee on an ex
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <param name="employee"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> AddEmployeeAsync(Employee employee, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(employee);
        ArgumentNullException.ThrowIfNull(transaction);

        IDbConnection? connection = transaction.Connection;

        if (connection is null)
        {
            throw new InvalidOperationException("No connection associated with transaction");
        }

        AddEmployee storedProcedure = new(employee);
        var command = storedProcedure.GetCommand(transaction, cancellationToken);

        int rows = await connection.ExecuteAsync(command);
        Debug.Assert(rows == 1);

        var id = storedProcedure.NewEmployeeId;
        Debug.Assert(id is not null);

        return storedProcedure.NewEmployeeId ?? 0; // should always be not null if parameter binding is correct
    }

    public async Task<int> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(employee);

        using var connection = await _connectionFactory.OpenDbConnectionAsync(cancellationToken);

        // could use implict transaction, but this just an example of using a transaction
        using var transaction = connection.BeginTransaction();

        try
        {
            int id = await AddEmployeeAsync(employee, transaction, cancellationToken);
            transaction.Commit();
            return id;
        }
        catch
        {
            transaction.Rollback();
            // log?
            throw;
        }
    }

    public async Task<Employee?> GetEmployee(int id, CancellationToken cancellationToken)
    {
        GetEmployees storedProcedure = new(id);
        var command = storedProcedure.GetCommand();

        using var connection = await _connectionFactory.OpenDbConnectionAsync(cancellationToken);

        Employee? employee = await connection.QuerySingleOrDefaultAsync<Employee>(command);
        return employee;
    }

    public async Task<IEnumerable<Employee>> GetEmployees(CancellationToken cancellationToken)
    {
        GetEmployees storedProcedure = new();
        var command = storedProcedure.GetCommand();

        using var connection = await _connectionFactory.OpenDbConnectionAsync(cancellationToken);

        IEnumerable<Employee> employees = await connection.QueryAsync<Employee>(command);

        return employees;
    }

    public async Task<IEnumerable<Employee>> GetEmployees(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        GetEmployees storedProcedure = new();
        var command = storedProcedure.GetCommand();

        using var connection = await _connectionFactory.OpenDbConnectionAsync(cancellationToken);

        IEnumerable<Employee> employees = await connection.QueryAsync<Employee>(command);

        return employees;
    }

}

