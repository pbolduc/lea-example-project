using Lea.Data.Models;
using System.Data;

namespace Lea.Data.Repositories;

public interface IEmployeeRepository : IRepository
{
    Task<int> AddEmployeeAsync(Employee employee, IDbTransaction transaction, CancellationToken cancellationToken);
    Task<int> AddEmployeeAsync(Employee employee, CancellationToken cancellationToken);
    Task<Employee?> GetEmployee(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Employee>> GetEmployees(CancellationToken cancellationToken);
    Task<IEnumerable<Employee>> GetEmployees(IEnumerable<int> ids, CancellationToken cancellationToken);
}
