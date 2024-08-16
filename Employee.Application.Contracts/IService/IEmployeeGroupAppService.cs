using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Entity;
using Volo.Abp.Application.Services;

namespace Application.Contracts.IService;

public interface IEmployeeGroupAppService:ICrudAppService<EmployeeGroupDto,int>,IApplicationService
{
    Task<EmployeeGroupDto> CreateGroup(CreateGroupDto create);

    Task<EmployeeGroupDto> UpdateGroup(int id,CreateGroupDto update);

    Task<bool> DestroyGroup(int groupId);

    Task<EntitiesResultDto<EmployeeGroupDto>> SearchGroups(SearchPageEmployeeGroupInput search);

    /// <summary>
    /// 获取所有分组及员工(含未分组员工)
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<List<EmployeeGroupDto>> GetGroupsAndEmployees(GetAllEmployeeGroupInput search);

    Task<List<EmployeeGroupDto>> GetAllGroups();


    Task<List<EmployeeGroupDto>> GetGroupsByIds(int[] ids);

    Task<List<EmployeeGroupBasicDto>> GetSimpleGroupsByIds(int[] ids);


    /// <summary>
    /// 根据组id获取员工
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    Task<EntitiesResultDto<EmployeeDto>> GetEmployeesOfGroup(int groupId);

    Task<EntitiesResultDto<EmployeeDto>> GetEmployeesOfGroupByPage(int groupId,SearchRequestInput input);

    /// <summary>
    /// 员工组内加入员工
    /// </summary>
    /// <param name="groupIds"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    Task<bool> AddEmployeeFromGroup(List<int> groupIds,int employeeId);

    /// <summary>
    /// 从员工组内删除员工
    /// </summary>
    /// <param name="groupIds"></param>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    Task<bool> DeleteEmployeeFromGroup(List<int> groupIds, int employeeId);
}