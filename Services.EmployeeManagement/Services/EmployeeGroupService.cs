using Domain.Entity;
using Domain.IRepository;
using Services.EmployeeManagement.Interface;
using System.ComponentModel.DataAnnotations;
using Domain.Dto.EmployeeGroup;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Collections.Generic;
using System.Linq;
using Domain.Dto;
using Domain.Dto.Employee;
using Microsoft.Extensions.Logging;
using Volo.Abp.Uow;

namespace Services.EmployeeManagement.Services;

public class EmployeeGroupService(
    IEmployeeRepository employeeRepository,
    IEmployeeGroupRepository groupRepository,
    IRepository<EmployeeInEmployeeGroup> employeeInGroupRepository,
    ILogger<EmployeeGroupService> logger) : IEmployeeGroupService
{
    public Task<EmployeeGroup> GetById(int id)
    {
        return groupRepository.GetAsync(id);
    }

    public Task<List<EmployeeGroup>> GetByIds(int[] ids)
    {
        return groupRepository.GetListAsync(x => ids.Contains(x.Id));
    }

    public async Task<EmployeeGroup> CreateGroup(CreateGroupDto group)
    {
        if (string.IsNullOrWhiteSpace(group.Description))
        {
            group.Description = null;
        }

        var count = await groupRepository.CountAsync(x => x.Name == group.Name);

        if (count == 0)
        {
            EmployeeGroup newgroup = new EmployeeGroup
            {
                Name = group.Name,
                Description = group.Description
            };
            return await groupRepository.InsertAsync(newgroup, true);
        }

        throw new BusinessException("500", "员工组名称已存在");
    }

    public async Task<EmployeeGroup> UpdateGroup(EmployeeGroup group)
    {
        var existingGroup = await groupRepository.AnyAsync(x => x.Name == group.Name);

        if (!existingGroup)
        {
            return await groupRepository.UpdateAsync(group);
        }

        throw new BusinessException("500", "员工组名称已存在");
    }

    public async Task<bool> DestroyGroup(int id)
    {
        var group = await groupRepository.GetAsync(id);
        var employeeLst = group.EmployeeInGroups.Select(x => x.Employee);
        if (!employeeLst.Any())
        {
            return await groupRepository.SoftDeleteAsync(x => x.Id == id);
        }

        throw new BusinessException("500", "员工组正被使用，禁止删除");
    }

    public async Task<(List<EmployeeGroup>, int)> SearchGroups(SearchPageEmployeeGroupInput search)
    {
        var query = await groupRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(search.Keyword))
        {
            query = query.Where(g => g.Name.Contains(search.Keyword) || g.Description.Contains(search.Keyword));
        }

        var totalCount = await query.CountAsync();

        var groups = await query
            .Skip(search.Offset)
            .Take(search.Limit)
            .ToListAsync();
        return (groups, totalCount);
    }

    public Task<List<EmployeeGroup>> GetAllGroups()
    {
        return  groupRepository.GetListAsync();
    }

    public async Task<Employee> JoinGroups(int employeeId, List<int> employeeGroupIds)
    {
        var employee = await employeeRepository.GetAsync(employeeId);
        if (employee == null) throw new BusinessException("400", "员工不存在");

        // 一次性获取员工已加入的组ID列表
        var joinedGroupIds = employee.EmployeeInGroups.Select(eg => eg.EmployeeGroup.Id).ToList();

        // 检查所有指定的组是否存在
        var existingGroups = await groupRepository.GetListAsync(g => employeeGroupIds.Contains(g.Id));
        var existingGroupIds = existingGroups.Select(g => g.Id).ToList();

        // 检查是否有不存在的组
        var notExistingGroupIds = employeeGroupIds.Except(existingGroupIds).ToList();
        if (notExistingGroupIds.Any())
        {
            throw new BusinessException("400", $"员工组[{string.Join(", ", notExistingGroupIds)}]不存在");
        }

        // 检查是否有重复加入的组
        var duplicateGroupIds = employeeGroupIds.Intersect(joinedGroupIds).ToList();
        if (duplicateGroupIds.Any())
        {
            var duplicateGroups = existingGroups.Where(g => duplicateGroupIds.Contains(g.Id)).Select(g => g.Name);
            throw new BusinessException("500", $"禁止重复加入员工组[{string.Join(", ", duplicateGroups)}]");
        }

        logger.LogDebug($"Adding employee {employeeId} to groups {string.Join(", ", employeeGroupIds)}");

        // 批量插入新的员工与组的关系
        var employeeInGroups = employeeGroupIds.Except(joinedGroupIds)
            .Select(groupId => new EmployeeInEmployeeGroup { EmployeeGroupId = groupId, EmployeeId = employeeId })
            .ToList();

        await employeeInGroupRepository.InsertManyAsync(employeeInGroups, true);

        return employee;
    }

    public async Task<Employee> LeaveGroups(int employeeId, List<int> employeeGroupIds)
    {
        var employee = await employeeRepository.GetAsync(employeeId);

        if (employee == null)
        {
            throw new BusinessException("400", "员工不存在");
        }

        // 获取需要移除的组
        var groupsToRemove = employee.EmployeeInGroups
            .Where(eg => employeeGroupIds.Contains(eg.EmployeeGroupId))
            .ToList();
        await employeeInGroupRepository.DeleteManyAsync(groupsToRemove);
        return employee;
    }

    public async Task<List<Employee>> GetEmployeesOfGroup(int groupId)
    {
        var group = await groupRepository.GetAsync(groupId);
        if (group == null)
        {
            throw new BusinessException("400", "员工组不存在");
        }
        var employeeList = group.EmployeeInGroups
            .Where(x => x.EmployeeGroupId == groupId)
            .Select(z=>z.Employee).ToList();
        return employeeList;
    }

    public async Task<(List<Employee>, int)> GetEmployeesOfGroupByPage(int id,SearchRequestInput input)
    {
        var group = await groupRepository.GetAsync(id);
        if (group == null)
        {
            throw new BusinessException("400", "员工组不存在");
        }
        var count = group.EmployeeInGroups
            .Where(x => x.EmployeeGroupId == id)
            .Select(x=>x.Employee).Count();
        var employeeGroup = group.EmployeeInGroups
            .Where(x => x.EmployeeGroupId == id)
            .Select(x => x.Employee).Skip(input.Offset).Take(input.Limit).ToList();
        return (employeeGroup, count);
    }
}