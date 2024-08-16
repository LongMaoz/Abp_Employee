using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Entity;
using Volo.Abp.Domain.Services;

namespace Services.EmployeeManagement.Interface;

public interface IEmployeeGroupService: IDomainService
{
    Task<EmployeeGroup> GetById(int id);

    Task<List<EmployeeGroup>> GetByIds(int[] ids);

    Task<EmployeeGroup> CreateGroup(CreateGroupDto group);

    Task<EmployeeGroup> UpdateGroup(EmployeeGroup group);

    Task<bool> DestroyGroup(int id);

    Task<(List<EmployeeGroup>,int)> SearchGroups(SearchPageEmployeeGroupInput search);

    Task<List<EmployeeGroup>> GetAllGroups();

    Task<Employee> JoinGroups(int employeeId, List<int> employeeGroupIds);

    Task<Employee> LeaveGroups(int employeeId, List<int> employeeGroupIds);


    /// <summary>
    /// 根据分组id获取员工
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    Task<List<Employee>> GetEmployeesOfGroup(int groupId);

    Task<(List<Employee>, int)> GetEmployeesOfGroupByPage(int id,SearchRequestInput input);
}