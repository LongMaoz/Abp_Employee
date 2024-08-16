using Domain.Dto.EmployeeRole;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Services.EmployeeManagement.Interface
{
    public interface IEmployeeRoleService:IDomainService
    {
        Task<EmployeeRole> CreateRoleAsync(EmployeeRole create);

        Task<EmployeeRole> UpdateRole(EmployeeRole update);

        Task DestroyRole(int id);

        Task<EmployeeRole> GetRoleById(int id);

        Task<EmployeeRole> GetRoleByName(string name);

        Task<List<EmployeeRole>> GetRolesByIds(int[] ids);

        Task<List<EmployeeRole>> GetAllRoles();

        Task<(List<EmployeeRole>,long)> ListRoles(int offset = 0,int limit = 10);

        Task<(List<EmployeeRole>,long)> SearchRoles(string? keyword,int offset = 0,int limit = 10);

        Task<bool> GrantRolesToEmployee(int employeeId, int[] roleIds);

        Task<List<bool>> GrantPermissionsToRole(int roleId,List<string> permissions);

        Task<List<string>> GetRolePermissions(int roleId);

        Task<List<Employee>> GetEmployeesForRole(int roleId);

        Task<List<EmployeeRole>> GetRolesForEmployee(int employeeId);
    }
}
