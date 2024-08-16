using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class ResetPasswordByForgetDto
{
    [EmailAddress(ErrorMessage = "电邮地址格式有误")]
    public string Email { get; set; }
    [Phone(ErrorMessage = "手机号码格式有误")]
    public string PhoneNumber { get; set; }
    public string VerificationCode { get; set; }
    public string Password { get; set; } 
}