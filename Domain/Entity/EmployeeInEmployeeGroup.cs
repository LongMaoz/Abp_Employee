using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Domain.Entity;

[Table("employee_in_employee_group")]
public class EmployeeInEmployeeGroup : global::Volo.Abp.Domain.Entities.Entity
{
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } // 导航属性

    public int EmployeeGroupId { get; set; }
    public virtual EmployeeGroup EmployeeGroup { get; set; } // 导航属性
    public override object[] GetKeys()
    {
        return new object[] { EmployeeId, EmployeeGroupId };
    }
}
