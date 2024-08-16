using Domain.Dto.EmployeeGroup;
using Domain.Dto.EmployeeRole;
using Domain.Entity;
using Domain.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Volo.Abp.Application.Dtos;

namespace Domain.Dto.Employee;

public class EmployeeDto : EntityDto<int>
{
    public string Avatar { get; set; }
    public int DefaultWarehouseId { get; set; } = 0;
    public string DisplayName { get; set; }
    public string Email { get; set; }

    public EntitiesResultDto<EmployeeGroupBasicDto> Groups => new()
    {
        Total = GroupsCount ?? GroupsList?.Count ?? 0,
        Data = GroupsList
    };

    public int? GroupsCount { get; set; }

    [JsonIgnore]
    public List<EmployeeGroupBasicDto> GroupsList { get; set; }

    public string Name { get; set; }
    //public string Password { get; set; }
    public string PhoneNumber { get; set; }

    public EntitiesResultDto<EmployeeRoleDto> Roles => new()
    {
        Total = RolesCount ?? RolesList?.Count ?? 0,
        Data = RolesList
    };

    public int? RolesCount { get; set; }

    [JsonIgnore]
    public List<EmployeeRoleDto> RolesList { get; set; }

    public EmployeeStatus Status { get; set; }
}