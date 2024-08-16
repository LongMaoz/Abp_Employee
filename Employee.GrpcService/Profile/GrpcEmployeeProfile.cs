using AutoMapper;
using Erp.Permissions;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Dto.EmployeeRole;

namespace Employee.GrpcService.Profile
{
    public class GrpcEmployeeProfile : AutoMapper.Profile
    {

        public GrpcEmployeeProfile()
        {
            CreateMap<EmployeeDto, Erp.Permissions.Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar ?? ""))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName ?? ""))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? ""))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(x => MapEmployeeBasicGroups(x.GroupsList)))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(x => MapEmployeeRoles(x.RolesList)));
            
            CreateMap<EmployeeDto, EmployeeSimple>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar ?? ""))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName ?? ""))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? ""))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<EmployeeGroupDto, EmployeeGroup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime.ToString() ?? ""))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime.ToString() ?? ""))
                .ForMember(dest=>dest.Employees, opt=>opt.MapFrom(x=> MapEmployeeBasic(x.EmployeesList)));

            CreateMap<EmployeeGroupDto, EmployeeGroupSimple>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name??""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime.ToString() ?? ""))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime.ToString() ?? ""));

            CreateMap<EmployeeGroupBasicDto, EmployeeGroup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime.ToString() ?? ""))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime.ToString() ?? ""));

            CreateMap<EmployeeGroupBasicDto, EmployeeGroupSimple>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime.ToString() ?? ""))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime.ToString() ?? ""));
            CreateMap<EmployeeRoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime.ToString() ?? ""))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime.ToString() ?? ""));
        }


        private static EmployeesResult MapEmployeeBasic(List<EmployeeBasicDto> employeeBasicList)
        {
            var result = new EmployeesResult();
            if (employeeBasicList != null)
            {
                result.Total = employeeBasicList.Count();
                result.Data.AddRange(employeeBasicList.Select(x => new Erp.Permissions.Employee()
                {
                    Avatar = x.Avatar ?? "",
                    DisplayName = x.DisplayName ?? "",
                    Email = x.Email ?? "",
                    Id = x.Id,
                    Name = x.Name ?? "",
                    PhoneNumber = x.PhoneNumber ?? "",
                    Status = x.Status.ToString()
                }));
            }
            else
            {
                result.Total = 0;
            }
            return result;
        }

        private static EmployeeGroupsResult MapEmployeeBasicGroups(List<EmployeeGroupBasicDto>? groupsList)
        {
            var result = new EmployeeGroupsResult();
            if (groupsList != null)
            {
                var list = groupsList.Select(x => new EmployeeGroup()
                {
                    CreateTime = x.CreateTime?.ToString() ?? "",
                    Description = x.Description ?? "",
                    Id = x.Id,
                    Name = x.Name ?? "",
                    UpdateTime = x.UpdateTime?.ToString() ?? ""
                }).ToList();
                result.Total = groupsList.Count();
                result.Data.AddRange(list);
            }
            else
            {
                result.Total = 0;
            }
            return result;
        }

        private static RolesResult MapEmployeeRoles(List<EmployeeRoleDto>? roleList)
        {
            var result = new RolesResult();
            if (roleList != null)
            {
                result.Total = roleList.Count();
                result.Data.AddRange(roleList.Select(x => new Role()
                {
                    CreateTime = x.CreateTime?.ToString() ?? "",
                    Description = x.Description ?? "",
                    Id = x.Id,
                    Name = x.Name ?? "",
                    UpdateTime = x.UpdateTime?.ToString() ?? ""
                }));
            }
            else
            {
                result.Total = 0;
            }
            return result;
        }
    }

}
