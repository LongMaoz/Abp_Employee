namespace Domain.Dto.Employee;

public class EmployeeQueryAllParameters
{
    public int GroupId { get; set; }

    public int RoleId { get; set; }

    public string Status { get; set; }

    public string Keyword { get; set; }
}