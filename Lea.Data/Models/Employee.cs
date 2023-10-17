using System.ComponentModel.DataAnnotations.Schema;

namespace Lea.Data.Models;

public class Employee
{
    public int? Id { get; set; }


    [Column("FIRST_NAME")]
    public string FirstName { get; set; }
}
