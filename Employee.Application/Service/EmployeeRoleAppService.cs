using Application.Contracts.IService;
using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeRole;
using Domain.Entity;
using Domain.IRepository;
using Services.EmployeeManagement.Interface;
using Volo.Abp.Service;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;
using static Domain.Message.EmployeeMessage;

namespace Application.Service;

public class EmployeeRoleAppService(
    IEmployeeRoleService roleService,
    IEmployeeRoleRepository roleRepository,
    ILogger<EmployeeAppService> logger,
    UnitAppManage unitAppManage
    ):CrudAppService<EmployeeRole,EmployeeRoleDto,int>(roleRepository),
    IEmployeeRoleAppService,
    IValidationEnabled
{
    public async Task<EmployeeRoleDto> CreateRole(CreateEmployeeRoleDto create)
    {
        EmployeeRole role = new EmployeeRole();
        ObjectMapper.Map(create, role);
        await unitAppManage.TranUnitOfWork(async () =>
        {
            await roleService.CreateRoleAsync(role);
            await roleService.GrantPermissionsToRole(role.Id, create.Permissions);
            return true;
        });
        return ObjectMapper.Map<EmployeeRole, EmployeeRoleDto>(role);
    }

    public async Task<EmployeeRoleDto> UpdateRole(CreateEmployeeRoleDto update)
    {
        EmployeeRole role = new EmployeeRole();
        ObjectMapper.Map(update, role);
        await unitAppManage.TranUnitOfWork(async () =>
        {
            await roleService.UpdateRole(role);
            await roleService.GrantPermissionsToRole(role.Id, update.Permissions);
            return true;
        });
        return await GetRoleById(role.Id);
    }
    
    public async Task<bool> DeleteRole(int roleId)
    {
        var result = await unitAppManage.TranUnitOfWork(async () =>
        {
            await roleService.DestroyRole(roleId);
            return true;
        });
        return result.Result;
    }

    public async Task<EntitiesResultDto<EmployeeRoleDto>> SearchRoles(SearchRequestInput search)
    {
        var result = await roleService.SearchRoles(search.Keyword,search.Offset,search.Limit);
        var dot = new EntitiesResultDto<EmployeeRoleDto>(result.Item2,ObjectMapper.Map<List<EmployeeRole>,List<EmployeeRoleDto>>(result.Item1));
        return dot;
    }

    public async Task<EmployeeRoleDto> GetRoleById(int id)
    {
        var role = await roleService.GetRoleById(id);
        var permission = await roleService.GetRolePermissions(id);
        var dto = ObjectMapper.Map<EmployeeRole,EmployeeRoleDto>(role);
        dto.Permissions = permission;
        return dto;
    }

    public async Task<List<EmployeeRoleDto>> GetRoleByIds(int[] ids)
    { 
        var role = await roleService.GetRolesByIds(ids);
        var dto = ObjectMapper.Map<List<EmployeeRole>,List<EmployeeRoleDto>>(role);
        return dto;
    }

    public async Task<List<EmployeeRoleDto>> GetAllRoles()
    {
        var roles = await roleService.GetAllRoles();
        return ObjectMapper.Map<List<EmployeeRole>,List<EmployeeRoleDto>>(roles);
    }

    public async Task<EntitiesResultDto<bool>> GrantPermissionsToRole(int id, List<string> permissions)
    {
        var grants = await roleService.GrantPermissionsToRole(id, permissions);
        return new EntitiesResultDto<bool>(grants.Count,grants);
    }

    public async Task<EntitiesResultDto<string>> GetRolePermissions(int id)
    {
        var permissions = await roleService.GetRolePermissions(id);
        return new EntitiesResultDto<string>(permissions.Count,permissions);
    }

    public async Task<EntitiesResultDto<EmployeeDto>> GetEmployeesForRole(int id)
    {
       var employeeLst = await roleService.GetEmployeesForRole(id);
        return new EntitiesResultDto<EmployeeDto>(employeeLst.Count,ObjectMapper.Map<List<Employee>,List<EmployeeDto>>(employeeLst));
    }

    public async Task<List<EmployeeRoleDto>> GetRolesForEmployee(int employeeId)
    {
        var roleLst = await roleService.GetRolesForEmployee(employeeId);

        var dtoLst = ObjectMapper.Map<List<EmployeeRole>,List<EmployeeRoleDto>>(roleLst);

        foreach (var item in dtoLst)
        {
            item.Permissions = await roleService.GetRolePermissions(item.Id);
        }
        return dtoLst;
    }
}