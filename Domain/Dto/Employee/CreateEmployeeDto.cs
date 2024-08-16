using Domain.Shared.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Domain.Dto.Employee;

public class CreateEmployeeDto:EntityDto<int>
{
    [Required(ErrorMessage ="姓名不能为空")]
    public string Name { get; set; }
    public string DisplayName { get; set; }

    [EmailAddress(ErrorMessage = "电邮地址格式有误")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "手机号码格式有误")]
    public string PhoneNumber { get; set; }
    public string Password { get; set; }

    [Url(ErrorMessage = "头像地址格式有误")]
    public string Avatar { get; set; }

    [Required(ErrorMessage = "状态不能为空")]
    public EmployeeStatus Status { get; set; }
    public int? DefaultWarehouseId { get; set; }

    [Required]
    public int[] RoleIds { get; set; } = new int[] { };

    public int[] GroupIds { get; set; } = new int[] { };
}