using Lea.Data.Models;
using Lea.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Lea.Data.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IDbConnectionFactory connectionFactory, IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AddEmployeesAsync(IList<Employee> employees, CancellationToken cancellationToken)
    {
        // add all the employees in a single database transaction
        using var connection = await _connectionFactory.OpenDbConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var ids = new List<int>();

        foreach (var employee in employees) 
        {
            var id = await _repository.AddEmployeeAsync(employee, transaction, cancellationToken);
            ids.Add(id);
        }

        transaction.Commit();

        // only update the passed in ids after transaction commit
        for (int i = 0; i < ids.Count; i++)
        {
            employees[i].Id = ids[i];
        }
    }
}
