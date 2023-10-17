using Lea.Data.Models;

namespace Lea.Data.Services;

public interface IEmployeeService
{
    Task AddEmployeesAsync(IList<Employee> employees, CancellationToken cancellationToken);
}
