using AutoMapper;
using Erp.Permissions;
using Grpc.Core;
using Application.Contracts.IService;

namespace Employee.GrpcService.Services;

public class GrpcEmployeeRoleService(IEmployeeRoleAppService roleAppService,IEmployeeAppService employeeAppService,IMapper mapper):RolesService.RolesServiceBase
{
    public override async Task<RolesResult> GetAllRoles(EmptyRequest request, ServerCallContext context)
    {
        var roleDtoLst = await roleAppService.GetAllRoles();
        RolesResult result = new RolesResult();
        result.Total = roleDtoLst.Count;
        result.Data.AddRange(mapper.Map<List<Role>>(roleDtoLst));
        return result;
    }

    public override async Task<PermissionsResult> GetEmployeePermissions(EmployeeById request, ServerCallContext context)
    {
        var employee = await employeeAppService.GetEmployeeById(request.Id);
        var permissions = employee.RolesList.Where(x=>x.Permissions != null).SelectMany(x=>x.Permissions).ToList();
        PermissionsResult result = new PermissionsResult();
        result.Total = permissions.Count;
        result.Data.AddRange(permissions);
        return result;
    }

    public override async Task<EmployeesResult> GetEmployeesForRole(RoleById request, ServerCallContext context)
    {
        var employeeDto = await roleAppService.GetEmployeesForRole(request.Id);
        EmployeesResult result = new EmployeesResult();
        result.Total = (int)employeeDto.Total;
        result.Data.AddRange(mapper.Map<List<Erp.Permissions.Employee>>(employeeDto.Data));
        return result;
    }

    public override async Task<Role> GetRoleById(RoleById request, ServerCallContext context)
    {
        var roleDto = await roleAppService.GetRoleById(request.Id);
        return mapper.Map<Role>(roleDto);
    }

    public override async Task<PermissionsResult> GetRolePermissions(RoleById request, ServerCallContext context)
    {
        var pageDto = await roleAppService.GetRolePermissions(request.Id);
        PermissionsResult result = new PermissionsResult();
        result.Total = (int)pageDto.Total;
        result.Data.AddRange(pageDto.Data);
        return result;
    }

    public override async Task<RolesResult> GetRolesByIds(RolesByIds request, ServerCallContext context)
    {
        var rolesDto = await roleAppService.GetRoleByIds(request.Ids.ToArray());
        RolesResult result = new RolesResult();
        result.Total = rolesDto.Count;
        result.Data.AddRange(mapper.Map<List<Role>>(rolesDto));
        return result;
    }

    public override async Task<RolesResult> GetRolesForEmployee(EmployeeById request, ServerCallContext context)
    {
        var employeeDto = await employeeAppService.GetEmployeeById(request.Id);
        RolesResult result = new RolesResult();
        result.Total = employeeDto.RolesList.Count;
        result.Data.AddRange(mapper.Map<List<Role>>(employeeDto.RolesList));
        return result;
    }

    public override async Task<RolesResult> ListRoles(ListRolesRequest request, ServerCallContext context)
    {
        var rolesDto = await roleAppService.SearchRoles(new Domain.Dto.SearchRequestInput() { Limit = request.Limit, Offset = request.Offset });
        RolesResult result = new RolesResult();
        result.Total = (int)rolesDto.Total;
        result.Data.AddRange(mapper.Map<List<Role>>(rolesDto.Data));
        return result;
    }

    public override async Task<RolesResult> SearchRoles(SearchRolesRequest request, ServerCallContext context)
    {
        var roles = await roleAppService.SearchRoles(new Domain.Dto.SearchRequestInput() { Limit = request.Limit, Offset = request.Offset, Keyword = request.Keyword });
        RolesResult result = new RolesResult();
        result.Total = (int)roles.Total;
        result.Data.AddRange(mapper.Map<List<Role>>(roles.Data));
        return result;  
    }
}