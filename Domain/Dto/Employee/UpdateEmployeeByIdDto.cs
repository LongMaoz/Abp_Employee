using Domain.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class UpdateEmployeeByIdDto
{
    [Required]
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Avatar { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public EmployeeStatus Status { get; set; }
    [Required]
    public int[] RoleIds { get; set; }
    public int[] GroupIds { get; set; }
    public int DefaultWarehouseId { get; set;}
}