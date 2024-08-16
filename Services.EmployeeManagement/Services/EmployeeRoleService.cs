using AutoMapper.Internal.Mappers;
using Casbin;
using Casbin.Rbac;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeRole;
using Domain.Entity;
using Domain.IRepository;
using EntityFrameworkCore.Repository;
using Services.EmployeeManagement.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Data;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Services.EmployeeManagement.Services;


public class EmployeeRoleService(IEnforcer enforcer, IEmployeeRoleRepository roleRepository, IEmployeeRepository employeeRepository) : IEmployeeRoleService
{
    public async Task<EmployeeRole> CreateRoleAsync(EmployeeRole role)
    {
        var count = await roleRepository.GetCountAsync(x => x.Name == role.Name);
        if (count > 0)
        {
            throw new BusinessException("500", "角色名称已存在");
        }
        role.UpdateTime = DateTime.UtcNow;
        return await roleRepository.InsertAsync(role, true);
    }

    public async Task<EmployeeRole> UpdateRole(EmployeeRole update)
    {
        var roleEntity = await roleRepository.GetAsync(update.Id);
        EmployeeRole roleNameEntity = null;
        try
        {
            roleNameEntity = await roleRepository.GetAsync(x => x.Name == update.Name);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        if (roleEntity == null)
        {
            throw new BusinessException("500", "角色不存在");
        }

        if (roleNameEntity != null && roleEntity.Id != roleNameEntity.Id)
        {
            throw new BusinessException("500", "角色名称已存在");
        }
        roleEntity.Description = update.Description;
        roleEntity.Name = update.Name;
        roleEntity.UpdateTime = DateTime.UtcNow;
        return await roleRepository.UpdateAsync(roleEntity, true);
    }

    public async Task DestroyRole(int id)
    {
        if (id == 0)
        {
            throw new BusinessException("500", "无法删除管理员角色");
        }
        var role = await roleRepository.GetAsync(id);
        var employees = enforcer.GetUsersForRole($"role_{role.Id}");
        if (!(employees?.Any() ?? false))
        {
            await roleRepository.SoftDeleteAsync(x => x.Id == id);
            await enforcer.DeleteRoleAsync($"role_{role.Id}");
        }
        else
        {
            throw new BusinessException("500", "该角色正被使用，禁止删除");
        }

    }

    public Task<EmployeeRole> GetRoleById(int id)
    {
        return roleRepository.GetAsync(id);
    }

    public Task<EmployeeRole> GetRoleByName(string name)
    {
        return roleRepository.GetAsync(x => x.Name == name);
    }

    public async Task<List<EmployeeRole>> GetRolesByIds(int[] ids)
    {
        if (ids == null || !ids.Any())
        {
            return new List<EmployeeRole>();
        }
        return await roleRepository.GetListAsync(x => ids.Contains(x.Id));
    }

    public async Task<List<EmployeeRole>> GetAllRoles()
    {
        return await roleRepository.GetListAsync();
    }

    public async Task<(List<EmployeeRole>, long)> ListRoles(int offset = 0, int limit = 10)
    {
        var count = await roleRepository.GetCountAsync();
        var list = await roleRepository.GetPagedListAsync(0, limit, "");
        return (list, count);
    }

    public async Task<(List<EmployeeRole>, long)> SearchRoles(string? keyword, int offset = 0, int limit = 10)
    {
        IQueryable<EmployeeRole> query = await roleRepository.GetQueryableAsync();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(role => EF.Functions.Like(role.Name, $"%{keyword}%")
                                        || EF.Functions.Like(role.Description, $"%{keyword}%"));
        }

        // 先获取总数
        int total = await query.CountAsync();

        List<EmployeeRole> roles = await query.Skip(offset).Take(limit).ToListAsync();

        return (roles, total);
    }


    public async Task<bool> GrantRolesToEmployee(int employeeId, int[] roleIds)
    {
        string employee = $"employee_{employeeId}";
        await enforcer.DeleteRolesForUserAsync(employee);
        var uniqueRoleIds = roleIds.Distinct().ToList();
        foreach (var role in from roleId in uniqueRoleIds where roleId != 0 select $"role_{roleId}")
        {
            await enforcer.AddRoleForUserAsync(employee, role);
        }
        return true;
    }

    public async Task<List<bool>> GrantPermissionsToRole(int roleId, List<string> permissions)
    {
        permissions = permissions.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
        string role = $"role_{roleId}";
        var deleteresult = await enforcer.DeletePermissionsForUserAsync(role);
        if (permissions == null || !permissions.Any())
        {
            return new List<bool>();
        }
        var addResults = new ConcurrentBag<bool>();
        foreach (var permission in permissions)
        {
            var result = await enforcer.AddPermissionForUserAsync(role, permission);
            addResults.Add(result);
        }
        //var tasks = permissions.Select(async permission =>
        //{
        //    var result = await enforcer.AddPermissionForUserAsync(role, permission);
        //    addResults.Add(result);
        //});
        //await Task.WhenAll(tasks);
        return addResults.ToList();
    }

    public async Task<List<string>> GetRolePermissions(int roleId)
    {
        string role = $"role_{roleId}";
        var permissions = enforcer.GetPermissionsForUser(role);
        var result = permissions
            .Where(p => p != null && p.Count() > 1) // 确保子集合非空且至少有两个元素
            .Select(p => p.ElementAt(1)) // 选择每个子集合的第二个元素
            .Where(p => !string.IsNullOrEmpty(p)) // 移除空字符串
            .Distinct() // 确保结果中的字符串唯一
            .ToList();
        return result;
    }

    public async Task<List<Employee>> GetEmployeesForRole(int roleId)
    {
        string role = $"role_{roleId}";

        // 获取角色对应的所有用户ID
        var userIds = enforcer.GetUsersForRole(role);

        // 提取员工ID
        var employeeIds = userIds
            .Select(x => x.StartsWith("employee_") ? int.Parse(x.Replace("employee_", "")) : (int?)null)
            .Where(x => x.HasValue) // 确保ID不是null
            .Select(x => x.Value) // 从Nullable<int>转换为int
            .ToList();

        // 根据员工ID从仓库中获取员工信息
        var employees = await employeeRepository.GetListAsync(x => employeeIds.Contains(x.Id));
        return employees;
    }

    public async Task<List<EmployeeRole>> GetRolesForEmployee(int employeeId)
    {
        var roles = enforcer.GetRolesForUser($"employee_{employeeId}");
        var roleIds = roles
            .Where(str => !string.IsNullOrEmpty(str) && str.StartsWith("role_"))
            .Select(str => int.TryParse(str.Replace("role_", ""), out var id) ? (int?)id : null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();

        List<EmployeeRole> roleList = new List<EmployeeRole>();
        if (roleIds != null && roleIds.Count > 0)
        {
            roleList = await roleRepository.GetListAsync(x => roleIds.Contains(x.Id));
        }
        return roleList;
    }
}