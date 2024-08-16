using Domain.DataFilter;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Domain.Entity;

[Table("Role")]
public class EmployeeRole:BasicAggregateRoot<int>, ICustomSoftDelete
{
    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public DateTime? DeleteTime { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}