using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class UpdateEmployeeDto
{
    //[Required]
    public string Name { get; set; }

    //[Required]
    [EmailAddress(ErrorMessage = "电邮地址格式有误")]
    public string Email { get; set; }

    //[Required]
    [Phone(ErrorMessage ="手机号码格式有误")]
    public string PhoneNumber { get; set; }

    public string DisplayName { get; set; }
    
    [Url(ErrorMessage = "头像地址格式有误")]
    public string Avatar { get; set; }
}