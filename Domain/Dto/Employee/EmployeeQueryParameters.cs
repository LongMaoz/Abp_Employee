using Domain.Shared.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Employee;

public class EmployeeQueryParameters
{
    [Required]
    [DefaultValue(0)]
    public int Offset { get; set; }

    [Required]
    [DefaultValue(10)]
    public int Limit { get; set; }

    public int GroupId { get; set; }

    public int RoleId { get; set; }

    public string Status { get; set; }

    public string Keyword { get; set; }

    public string Name { get; set; }
}