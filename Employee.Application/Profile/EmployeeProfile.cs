using Autofac.Core;
using AutoMapper;
using Casbin;
using Casbin.Rbac;
using Application.Contracts.IService;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Dto.EmployeeRole;
using Domain.Entity;
using Domain.IRepository;
using Domain.Message.Employee;
using Domain.Shared.Extends;

namespace Application.Profile;

public class EmployeeProfile: AutoMapper.Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeBasicDto>().ReverseMap();
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dto => dto.GroupsList, opt => opt.MapFrom(entity => entity.EmployeeInGroups.Where(x => x.EmployeeGroup != null).Select(x => x.EmployeeGroup).ToList()))
            .ForMember(dto => dto.RolesList, opt => opt.MapFrom(entity => entity.RolesList))
            .AfterMap((entity, dto) =>
            {
                dto.Groups.Total = dto.GroupsList?.Count ?? 0;
                dto.Roles.Total = dto.RolesList?.Count ?? 0;
            });

        CreateMap<EmployeeDto, Employee>();
        CreateMap<EmployeeBasicDto, EmployeeDto>().ReverseMap();

        CreateMap<EmployeeGroup, EmployeeGroupDto>()
            .ForMember(dto=>dto.EmployeesList,
            opt => opt.MapFrom(entity=>entity.EmployeeInGroups.Select(z=>z.Employee)))
            .AfterMap((entity, dto) =>
            {
                dto.EmployeesCount = dto.EmployeesList?.Count ?? 0;
            });
        CreateMap<EmployeeGroupDto, EmployeeGroup>();

        CreateMap<EmployeeGroup, EmployeeGroupBasicDto>();
        CreateMap<EmployeeGroupDto, EmployeeGroupBasicDto>().ReverseMap();

        CreateMap<EmployeeRole, EmployeeRoleDto>().ReverseMap();
        
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<EmployeeDto, EmployeeMessageInfo>().ForMember(dto=>dto.Status,opt=>opt.MapFrom(entity=> entity.Status.GetString()));


        CreateMap<CreateEmployeeRoleDto, EmployeeRole>()
            .ForMember(entity=> entity.CreateTime,
            opt=>opt.MapFrom(e=> DateTime.UtcNow));
    }
}
