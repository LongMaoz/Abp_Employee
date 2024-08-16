using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class ForgetPasswordDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    [Required]
    public string VerificationCode { get; set; }
    [Required]
    public string Password { get; set; }
}