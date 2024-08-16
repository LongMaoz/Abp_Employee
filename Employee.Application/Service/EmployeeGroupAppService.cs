using Application.Contracts.IService;
using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Entity;
using Domain.IRepository;
using Domain.Message;
using Domain.Shared.Enums;
using EventBus.Publish.Interface;
using Services.EmployeeManagement.Interface;
using Volo.Abp.Service;
using System.Collections.Generic;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Service;

public class EmployeeGroupAppService(IEmployeeGroupService service,
    IEmployeeService employeeService,
    IEmployeeGroupRepository groupRepository,
    IEmployeeEventPublic employeeEvent,
    UnitAppManage unitAppManage)
    : CrudAppService<EmployeeGroup, EmployeeGroupDto, int>(groupRepository), IEmployeeGroupAppService, IValidationEnabled
{
    public async Task<EmployeeGroupDto> CreateGroup(CreateGroupDto create)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(async () =>
        {
            var newGroup = await service.CreateGroup(create);
            return newGroup;
        });
        return ObjectMapper.Map<EmployeeGroup, EmployeeGroupDto>(unitResult.Value);
    }

    public async Task<EmployeeGroupDto> UpdateGroup(int id, CreateGroupDto update)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(async () =>
        {
            var group = await service.GetById(id);
            group.Description = update.Description;
            group.Name = update.Name;
            var newGroup = await service.UpdateGroup(group);
            return newGroup;
        });
        return ObjectMapper.Map<EmployeeGroup, EmployeeGroupDto>(unitResult.Value);
    }

    public async Task<bool> DestroyGroup(int groupId)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(() => service.DestroyGroup(groupId));
        return unitResult.Value;
    }

    public async Task<EntitiesResultDto<EmployeeGroupDto>> SearchGroups(SearchPageEmployeeGroupInput search)
    {
        var result = await service.SearchGroups(search);
        List<EmployeeGroupDto> Dtos = new List<EmployeeGroupDto>();
        if (search.ReturnEmployee is ReturnOption.No)
        {
            var basic = ObjectMapper.Map<List<EmployeeGroup>, List<EmployeeGroupBasicDto>>(result.Item1);
            Dtos = ObjectMapper.Map<List<EmployeeGroupBasicDto>, List<EmployeeGroupDto>>(basic);
        }
        else
        {
            //这里会根据映射自动从导航属性加载EmployeeList
            Dtos = ObjectMapper.Map<List<EmployeeGroup>, List<EmployeeGroupDto>>(result.Item1);
        }
        return new EntitiesResultDto<EmployeeGroupDto>
        {
            Data = Dtos,
            Total = result.Item2
        };
    }

    public async Task<List<EmployeeGroupDto>> GetGroupsAndEmployees(GetAllEmployeeGroupInput search)
    {
        var groups = await service.GetAllGroups();

        var groupDtos = ObjectMapper.Map<List<EmployeeGroup>, List<EmployeeGroupDto>>(groups);
        
        //获取未分组员工
        if (search.ReturnUngrouped.HasValue) {
            var unGroup = await employeeService.GetEmployeeNotGroup();
            var dto = new EmployeeGroupDto()
            {
                Id = 0,
                Name = "未分组",
                Description = "展示所有未分组的员工",
                EmployeesList = ObjectMapper.Map<List<Employee>, List<EmployeeBasicDto>>(unGroup)
            };
            groupDtos.AddFirst(dto);
        }

        if (search.Status.HasValue)
        {

            foreach (var item in groupDtos)
            {
                item.EmployeesList = item.EmployeesList?.Where(x => x.Status == search.Status).ToList();
            }
        }

        return groupDtos;
    }

    public async Task<List<EmployeeGroupDto>> GetAllGroups()
    {
        var employeeLst = await service.GetAllGroups();
        return ObjectMapper.Map<List<EmployeeGroup>,List<EmployeeGroupDto>>(employeeLst);
    }

    public async Task<List<EmployeeGroupDto>> GetGroupsByIds(int[] ids)
    {
        var employeeLst = await service.GetByIds(ids);
        return ObjectMapper.Map<List<EmployeeGroup>, List<EmployeeGroupDto>>(employeeLst);
    }

    public async Task<List<EmployeeGroupBasicDto>> GetSimpleGroupsByIds(int[] ids)
    {
        var employeeLst = await service.GetByIds(ids);
        return ObjectMapper.Map<List<EmployeeGroup>, List<EmployeeGroupBasicDto>>(employeeLst);
    }

    public async Task<EntitiesResultDto<EmployeeDto>> GetEmployeesOfGroup(int groupId)
    {
        var groupList = await service.GetEmployeesOfGroup(groupId);
        var lst = ObjectMapper.Map<List<Employee>, List<EmployeeBasicDto>>(groupList);
        var dtoList = ObjectMapper.Map<List<EmployeeBasicDto>, List<EmployeeDto>>(lst);
        return new EntitiesResultDto<EmployeeDto>(dtoList.Count, dtoList);
    }

    public async Task<EntitiesResultDto<EmployeeDto>> GetEmployeesOfGroupByPage(int groupId,SearchRequestInput input)
    {
        var result = await service.GetEmployeesOfGroupByPage(groupId, input);
        var groupList = ObjectMapper.Map<List<Employee>, List<EmployeeBasicDto>>(result.Item1);
        var dtoList = ObjectMapper.Map<List<EmployeeBasicDto>, List<EmployeeDto>>(groupList);
        return new EntitiesResultDto<EmployeeDto>()
        {
            Total = result.Item2,
            Data = dtoList
        };
    }


    public async Task<bool> AddEmployeeFromGroup(List<int> groupIds, int employeeId)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(() => service.JoinGroups(employeeId, groupIds));
        var employee = unitResult.Value;
        //发送消息
        await employeeEvent.EmployeeUpdateCompleteAsync(ObjectMapper.Map<Employee, EmployeeDto>(employee), new Operator() { ID = employeeId, Name = "当前登录用户" });
        return true;
    }

    public async Task<bool> DeleteEmployeeFromGroup(List<int> groupIds, int employeeId)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(() => service.LeaveGroups(employeeId, groupIds));
        var employee = unitResult.Value;
        //发送消息
        await employeeEvent.EmployeeUpdateCompleteAsync(ObjectMapper.Map<Employee, EmployeeDto>(employee), new Operator() { ID = employeeId, Name = "当前登录用户" });
        return true;
    }
}