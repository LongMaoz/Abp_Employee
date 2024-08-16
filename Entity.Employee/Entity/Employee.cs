using IRentals.Entity.EmployeeManagement.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace IRentals.Entity.EmployeeManagement.Entity;

[Table("employee")]
public class Employee: BasicAggregateRoot<int>
{

    public Employee()
    {
        Status = EmployeeStatus.Disabled;
    }

    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string Avatar { get; set; }
    public EmployeeStatus Status { get; set; }
    public int? DefaultWarehouseID { get; set; }
}