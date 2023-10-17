using Dapper.Oracle;
using System.Data;

namespace Lea.Data.StoredProcedures;

public class GetEmployees : StoredProcedure
{
    private readonly int[]? _ids;

    public GetEmployees() : base("HR_PACKAGE.prGetEmployees")
    {
    }

    public GetEmployees(int id) : this(new int[] { id })
    {
    }

    public GetEmployees(IEnumerable<int> ids) : this()
    {
        ArgumentNullException.ThrowIfNull(ids);
        _ids = ids.ToArray();
    }

    public IEnumerable<int> Ids
    {
        get
        {
            if (_ids is null)
            {
                return Enumerable.Empty<int>();
            }

            return new List<int>(_ids);
        }
    }

    protected override void AddParameters(OracleDynamicParameters parameters)
    {
        if (_ids is not null)
        {
            // not tested - see https://github.com/DIPSAS/Dapper.Oracle/tree/main
            parameters.ArrayBindCount = _ids.Length;
            parameters.Add(":id", _ids, OracleMappingType.Int32, ParameterDirection.Input);
        }

        parameters.Add(":p_rc", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
    }
}
