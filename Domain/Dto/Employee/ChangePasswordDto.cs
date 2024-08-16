using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class ChangePasswordDto
{
    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }
}