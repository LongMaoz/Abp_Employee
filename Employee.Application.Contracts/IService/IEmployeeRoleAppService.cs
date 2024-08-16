using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeRole;
using Volo.Abp.Application.Services;

namespace Application.Contracts.IService;

public interface IEmployeeRoleAppService:ICrudAppService<EmployeeRoleDto,int>,IApplicationService
{

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<EmployeeRoleDto> CreateRole(CreateEmployeeRoleDto create);

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    Task<EmployeeRoleDto> UpdateRole(CreateEmployeeRoleDto update);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<bool> DeleteRole(int roleId);

    Task<EntitiesResultDto<EmployeeRoleDto>> SearchRoles(SearchRequestInput search);

    Task<EmployeeRoleDto> GetRoleById(int id);

    Task<List<EmployeeRoleDto>> GetRoleByIds(int[] ids);

    Task<List<EmployeeRoleDto>> GetAllRoles();

    Task<EntitiesResultDto<bool>> GrantPermissionsToRole(int id,List<string> permissions);

    Task<EntitiesResultDto<string>> GetRolePermissions(int id);
    
    Task<EntitiesResultDto<EmployeeDto>> GetEmployeesForRole(int id);

    Task<List<EmployeeRoleDto>> GetRolesForEmployee(int employeeId);
}