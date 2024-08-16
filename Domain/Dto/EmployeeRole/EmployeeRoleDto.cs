using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Domain.Dto.EmployeeRole;

public class EmployeeRoleDto : EntityDto<int>
{
    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public DateTime?  DeleteTime { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<string> Permissions { get; set; }
}