namespace Domain.Dto.EmployeeRole;

public class GrantRolesToEmployeeRequestDto
{
    public int EmployeeId { get; set; }

    public int[] RoleIds { get; set; }
}