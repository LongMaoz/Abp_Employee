using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class SendVerifyCodeDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
}