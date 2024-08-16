using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace IRentals.Entity.EmployeeManagement.Entity
{
    [Table("employee_group")]
    public class EmployeeGroup : BasicAggregateRoot<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
