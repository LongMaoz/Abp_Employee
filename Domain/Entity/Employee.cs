using System.ComponentModel.DataAnnotations.Schema;
using Domain.DataFilter;
using Domain.Shared.Enums;
using Volo.Abp.Domain.Entities;

namespace Domain.Entity;

[Table("employee")]
public class Employee:BasicAggregateRoot<int>, ICustomSoftDelete
{
    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }

    public DateTime? DeleteTime { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string Avatar { get; set; }

    public EmployeeStatus Status { get; set; }

    public int? DefaultWarehouseId { get; set; }

    //导航属性
    public List<EmployeeGroup> GroupsList { get; set; }

    public virtual ICollection<EmployeeInEmployeeGroup> EmployeeInGroups { get; set; }

    public List<EmployeeRole> RolesList { get; set; }
}
