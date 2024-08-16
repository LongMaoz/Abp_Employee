using Domain.DataFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Domain.Entity
{
    [Table("employee_group")]
    public class EmployeeGroup : BasicAggregateRoot<int>, ICustomSoftDelete
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateTime { get; set; } = DateTime.UtcNow;
        public DateTime? DeleteTime { get; set; }

        public virtual ICollection<EmployeeInEmployeeGroup> EmployeeInGroups { get; set; }
    }
}
