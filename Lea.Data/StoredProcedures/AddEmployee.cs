using Dapper.Oracle;
using Lea.Data.Models;
using System.Data;

namespace Lea.Data.StoredProcedures;

public class AddEmployee : StoredProcedure
{
    private readonly Employee _employee;

    public AddEmployee(Employee employee) : base("HR_PACKAGE.prAddEmployee")
    {
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
    }

    protected override void AddParameters(OracleDynamicParameters parameters)
    {
        // TODO: find a way to avoid binding each property explicitly: attributes, relfection?

        parameters.Add("first_name", _employee.FirstName, OracleMappingType.NVarchar2, ParameterDirection.Input, size: 255);

        parameters.Add(":id", null, OracleMappingType.Int32, ParameterDirection.Output);
    }

    /// <summary>
    /// Gets the output parameter of the new employee id
    /// </summary>
    public int? NewEmployeeId => Parameters.Get<int?>(":id");
}
