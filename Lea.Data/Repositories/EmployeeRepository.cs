using Dapper;
using Lea.Data.Models;
using Lea.Data.StoredProcedures;
using System.Data;
using System.Diagnostics;

namespace Lea.Data.Repositories;

public class EmployeeRepository : Repository, IEmployeeRepository
{
    public EmployeeRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
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
               
        AddEmployee command = new(employee);

        int rows = await ExecuteAsync(command, cancellationToken);
        Debug.Assert(rows == 1);

        var id = command.NewEmployeeId;
        Debug.Assert(id is not null);

        return id ?? 0; // should always be not null if parameter binding is correct
    }

    public async Task<int> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(employee);

        using var connection = await ConnectionFactory.OpenDbConnectionAsync(cancellationToken);

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
        GetEmployees command = new(id);

        Employee? employee = await QuerySingleOrDefaultAsync<Employee>(command, cancellationToken);

        return employee;
    }

    public async Task<IEnumerable<Employee>> GetEmployees(CancellationToken cancellationToken)
    {
        GetEmployees command = new();

        var employees = await QueryAsync<Employee>(command, cancellationToken);

        return employees;
    }

    public async Task<IEnumerable<Employee>> GetEmployees(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        GetEmployees command = new(ids);

        var employees = await QueryAsync<Employee>(command, cancellationToken);

        return employees;
    }
}
