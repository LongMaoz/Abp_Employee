using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Domain.Dto.EmployeeRole;

public class CreateEmployeeRoleDto:EntityDto<int>
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 权限数组
    /// </summary>
    public List<string> Permissions { get; set; }
}