using Domain.Dto.Employee;
using Domain.Entity;
using Domain.IRepository;
using Services.EmployeeManagement.Interface;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Casbin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Shared.Enums;

namespace Services.EmployeeManagement.Services;

public class EmployeeService(IEmployeeRepository employeeRepository,
    IEmployeeRoleService  employeeRoleService,
    IRepository<EmployeeInEmployeeGroup> employeeInGroupRepository,
    ILogger<EmployeeService> logger,
    IEnforcer enforcer) 
    : DomainService, IEmployeeService
{
    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeDto input)
    {
        var isExist = await employeeRepository.AnyAsync(x=>x.Name == input.Name);
        if (isExist)
        {
            throw new BusinessException("500", "员工名已存在");
        }
        Employee employee = new Employee()
        {
            Avatar = input.Avatar,
            DisplayName = input.DisplayName,
            Email = input.Email,
            Name = input.Name,
            PhoneNumber = input.PhoneNumber,
            Status = input.Status,
            Password = input.Password,
            DefaultWarehouseId = input.DefaultWarehouseId,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };
        var resultEntity = await employeeRepository.InsertAsync(employee,true);
        return resultEntity;
    }

    public async Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto input)
    {
        var employee = await employeeRepository.GetAsync(id);
        // 检查电邮地址或手机号码与现有员工是否有冲突
        var isEmailOrPhoneExists = await employeeRepository
            .AnyAsync(e => e.Id != id && (e.Email == input.Email || e.PhoneNumber == input.PhoneNumber));
        if (isEmailOrPhoneExists)
        {
            throw new BusinessException("500", "电邮地址或手机号码与现有员工有冲突");
        }

        // 更新员工简况
        if (input.Email != null)
        {
            employee.Email = input.Email;
        }
        if (input.PhoneNumber != null)
        {
            employee.PhoneNumber = input.PhoneNumber;
        }
        if (input.Avatar != null)
        {
            employee.Avatar = input.Avatar;
        }
        if (input.DisplayName != null)
        {
            employee.DisplayName = input.DisplayName;
        }

        if (input.Name != null)
        {
            employee.Name = input.Name;
        }
        return await employeeRepository.UpdateAsync(employee);
    }

    public async Task<Employee> UpdateEmployeeByIdAsync(int id, CreateEmployeeDto input)
    {
        var employee = await employeeRepository.GetAsync(id);
        // 检查电邮地址或手机号码与现有员工是否有冲突
        var isEmailOrPhoneExists = await employeeRepository
            .AnyAsync(e => e.Id != id && (e.Email == input.Email || e.PhoneNumber == input.PhoneNumber));
        if (isEmailOrPhoneExists)
        {
            throw new BusinessException("500", "电邮地址或手机号码与现有员工有冲突");
        }
        // 更新员工简况
        employee.Email = input.Email;
        employee.Name = input.Name;
        employee.PhoneNumber = input.PhoneNumber;
        employee.Avatar = input.Avatar;
        employee.DisplayName = input.DisplayName;
        return await employeeRepository.UpdateAsync(employee, true);
    }

    public async Task<Employee> UpdateEmployeeStatus(int id, EmployeeStatus status)
    {
        if (!(await employeeRepository.AnyAsync(x=>x.Id == id)))
        {
            throw new BusinessException("500",$"员工id:{id}不存在。");
        }
        if(await employeeRepository.UpdateBulkAsync(x => x.Id == id,
            x => x.SetProperty(z => z.Status, status)))
        {
            return await this.GetEmployeeById(id);
        }
        return null;
    }

    public async Task<Employee> UpdateEmployeePassword(int id, string password)
    {
        var employee = await employeeRepository.GetAsync(id);
        employee.Password = password;
        return await employeeRepository.UpdateAsync(employee,true);
    }

    public async Task LeaveGroups(int employeeId, IEnumerable<int> employeeGroupIds)
    {
        var employee = await employeeRepository.GetAsync(x => x.Id == employeeId);
        if (employee == null)
        {
            throw new BusinessException("500",$"员工id:{employeeId}不存在");
        }

        // 从员工的群组列表中移除这些群组
        await employeeInGroupRepository.DeleteAsync(x => x.EmployeeId == employeeId && employeeGroupIds.Contains(x.EmployeeGroupId),true);
    }

    public async Task JoinGroups(int employeeId, List<int> employeeGroupIds)
    {
        var currentGroups = await employeeInGroupRepository.GetListAsync(e => e.EmployeeId == employeeId);

        foreach (var employeeGroupId in employeeGroupIds)
        {
            if (currentGroups.Any(g => g.EmployeeGroupId == employeeGroupId))
            {
                throw new BusinessException("500", $"禁止重复加入员工组 {employeeGroupId}.");
            }

            var employeeGroup = new EmployeeInEmployeeGroup
            {
                EmployeeId = employeeId,
                EmployeeGroupId = employeeGroupId
            };
            await employeeInGroupRepository.InsertAsync(employeeGroup,true);
            logger.LogDebug($"员工 {employeeId} 加入员工组 {employeeGroupId}.");
        }
    }

    public async Task<bool> DestroyEmployee(int employeeId)
    {
        // 移除角色 删除员工
        await enforcer.DeleteRolesForUserAsync($"employee_${employeeId}");
        await employeeRepository.SoftDeleteAsync(x => x.Id == employeeId);
        return true;
    }

    public async Task<(List<Employee>, int)> SearchEmployees(EmployeeQueryParameters parameters)
    {
        var query = await employeeRepository.GetQueryableAsync();

        if (parameters.RoleId!=0)
        {
            var employeeIds = enforcer.GetUsersForRole($"role_{parameters.RoleId}")
                .Select(x => x.StartsWith("employee_") ? int.Parse(x.Replace("employee_", "")) : (int?)null)
                .Where(x => x.HasValue)
                .ToList();

            if (employeeIds.Any())
            {
                query = query.Where(e => employeeIds.Contains(e.Id));
            }
            else
            {
                return (new List<Employee>(), 0);
            }
        }
        if (parameters.GroupId!=0)
        {
            query = query.Where(e => e.EmployeeInGroups.Any(g => g.EmployeeGroupId == parameters.GroupId));
        }

        if (!string.IsNullOrEmpty(parameters.Status) && Enum.TryParse<EmployeeStatus>(parameters.Status, ignoreCase: true, out var statusEnum))
        {
            query = query.Where(e => e.Status == statusEnum);
        }
        else if(!string.IsNullOrEmpty(parameters.Status))
        {
            throw new BusinessException("500","提供的状态无效。");
        }

        if (!string.IsNullOrEmpty(parameters.Keyword))
        {
            query = query.Where(e => e.Name.Contains(parameters.Keyword) || e.DisplayName.Contains(parameters.Keyword) || e.Email.Contains(parameters.Keyword) || e.PhoneNumber.Contains(parameters.Keyword));
        }

        if (!string.IsNullOrEmpty(parameters.Name))
        {
            query = query.Where(e => e.Name.Contains(parameters.Name));
        }

        var total = await query.CountAsync();

        var employees = await query
            .OrderByDescending(e => e.Id)
            .Skip(parameters.Offset)
            .Take(parameters.Limit)
            .ToListAsync();

        foreach (var item in employees)
        {
            var roleLst = await employeeRoleService.GetRolesForEmployee(item.Id);
            item.RolesList = roleLst;
        }

        return (employees, total);
    }

    public async Task<List<Employee>> SearchSimpleEmployeesNoPage(EmployeeQueryAllParameters parameters)
    {
        var result = await this.SearchSimple(parameters.GroupId, parameters.RoleId, parameters.Status==null?null:Enum.Parse<EmployeeStatus>(parameters.Status), parameters.Keyword);
        return result.Item1;
    }

    public async Task<Employee> GetEmployeeById(int id)
    {
        var employee = await employeeRepository.GetAsync(id);
        var roleLst = await employeeRoleService.GetRolesForEmployee(employee.Id);
        employee.RolesList = roleLst;
        return employee;
    }

    public async Task<List<Employee>> GetEmployeeByIds(int[] ids)
    {
        var employees = await employeeRepository.GetListAsync(x => ids.Contains(x.Id));

        foreach (var item in employees)
        {
            var roleLst = await employeeRoleService.GetRolesForEmployee(item.Id);
            item.RolesList = roleLst;
        }
        return employees;
    }

    public async Task<Employee> GetEmployeeByEmail(string email)
    {
        var employee = await employeeRepository.GetAsync(x => x.Email == email);
        var roleLst = await employeeRoleService.GetRolesForEmployee(employee.Id);
        employee.RolesList = roleLst;
        return employee;
    }

    public async Task<Employee> GetEmployeeByPhoneNumber(string phoneNumber)
    {
        var employee = await employeeRepository.GetAsync(x => x.PhoneNumber == phoneNumber);
        var roleLst = await employeeRoleService.GetRolesForEmployee(employee.Id);
        employee.RolesList = roleLst;
        return employee;
    }

    public async Task<List<Employee>> GetEmployeeNotGroup()
    {
        return await employeeRepository.GetListAsync(x => !x.EmployeeInGroups.Any() || x.EmployeeInGroups.Any(z=>z.EmployeeGroup == null));
    }

    private async Task<(List<Employee>, int?)> SearchSimple(
        int? groupId = null,
        int? roleId = null,
        EmployeeStatus? status = null,
        string keyword = null,
        bool paging = false,
        int offset = 0,
        int limit = 10)
    {
        // 构建基础查询
        var query = await employeeRepository.GetQueryableAsync();

        if (roleId.HasValue)
        {
            // 从数据库中读取策略
            var employeeIds = enforcer.GetUsersForRole($"role_{roleId}")
                .Select(x => x.StartsWith("employee_") ? int.Parse(x.Replace("employee_", "")) : (int?)null)
                .Where(x => x.HasValue)
                .ToList();

            if (employeeIds.Any())
            {
                query = query.Where(e => employeeIds.Contains(e.Id));
            }
            else
            {
                return (new List<Employee>(), 0);
            }
        }

        if (groupId.HasValue)
        {
            query = query.Where(e => e.GroupsList.Any(g => g.Id == groupId.Value));
        }

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(e => e.Name.Contains(keyword) || e.DisplayName.Contains(keyword) || e.Email.Contains(keyword) || e.PhoneNumber.Contains(keyword));
        }

        if (paging)
        {
            var total = await query.CountAsync();
            var employees = await query
                .OrderByDescending(e => e.Id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            foreach (var item in employees)
            {
                var roleLst = await employeeRoleService.GetRolesForEmployee(item.Id);
                item.RolesList = roleLst;
            }
            return (employees, total);
        }
        else
        {
            var employees = await query
                .OrderByDescending(e => e.Id)
                .ToListAsync();
            foreach (var item in employees)
            {
                var roleLst = await employeeRoleService.GetRolesForEmployee(item.Id);
                item.RolesList = roleLst;
            }
            return (employees, null);
        }
    }

}