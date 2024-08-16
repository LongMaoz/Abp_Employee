using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace IRentals.Entity.EmployeeManagement.Entity;

[Table("employee_in_employee_group")]
public class EmployeeInEmployeeGroup : global::Volo.Abp.Domain.Entities.Entity
{
    public int EmployeeId { get; set; }

    public int EmployeeGroupId { get; set; }

    public override object?[] GetKeys()
    {
        return new object[] { EmployeeId, EmployeeGroupId };
    }
}