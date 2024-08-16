namespace Domain.Dto.EmployeeRole;

public class GrantPermissionsToRoleRequestDto
{
    public int RoleId { get; set; }

    public string[] Permissions { get; set; }
}