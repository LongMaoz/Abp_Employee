using AutoMapper;
using Erp.Permissions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Application.Contracts.IService;
using Domain.Dto.Employee;
using Domain.Shared.Enums;
using Employee.GrpcService;
using Erp.Permissions;
using Microsoft.Extensions.Logging;

namespace Employee.GrpcService.Services
{
    public class GrpcEmployeesService(
        ILogger<GrpcEmployeesService> logger,
        IEmployeeAppService employeeApp,
        IMapper mapper
        ) 
        : EmployeesService.EmployeesServiceBase
    {
        public override Task<Empty> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        public override async Task<Erp.Permissions.Employee> CreateEmployee(Erp.Permissions.Employee request, ServerCallContext context)
        {
            CreateEmployeeDto createEmployeeDto = new CreateEmployeeDto()
            {
                Name = request.Name,
                Avatar = request.Avatar,
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Status = System.Enum.Parse<EmployeeStatus>(request.Status),
                RoleIds = request.Roles.Data.Select(x=>x.Id).ToArray(),
                GroupIds = request.Groups.Data.Select(x=>x.Id).ToArray(),
            };
            var dto = await employeeApp.CreateEmployee(createEmployeeDto, null);
            return mapper.Map<EmployeeDto,Erp.Permissions.Employee>(dto);
        }

        public override async Task<Empty> DestroyEmployee(EmployeeById request, ServerCallContext context)
        {
            await employeeApp.DeleteEmployeeById(request.Id, new Domain.Message.Operator() { ID = 0,Name =""});
            return new Empty();
        }

        public override Task<Empty> ForgetPasswordByEmail(StringValue request, ServerCallContext context)
        {
            return base.ForgetPasswordByEmail(request, context);
        }

        public override async Task<Erp.Permissions.Employee> GetEmployeeById(EmployeeById request, ServerCallContext context)
        {
            var dto = await employeeApp.GetEmployeeById(request.Id);
            return mapper.Map<EmployeeDto, Erp.Permissions.Employee>(dto);
        }

        public override Task<Empty> ForgetPasswordByPhoneNumber(StringValue request, ServerCallContext context)
        {
            return base.ForgetPasswordByPhoneNumber(request, context);
        }
        
        public override async Task<EmployeesResult> GetEmployeesByIds(EmployeesByIds request, ServerCallContext context)
        {
            var dtoLst = await employeeApp.GetEmployeeByIds(request.Ids.ToArray());
            var employeeLst = mapper.Map<List<Erp.Permissions.Employee>>(dtoLst);
            var result = new EmployeesResult();
            result.Data.AddRange(employeeLst);
            result.Total = employeeLst.Count;
            return result;
        }

        public override async Task<EmployeeGroupsSimpleResult> GetGroupsById(EmployeeById request, ServerCallContext context)
        {
            var dtoLst = await employeeApp.GetEmployeeGroups(request.Id);
            var groupLst = mapper.Map<List<EmployeeGroupSimple>>(dtoLst);
            var result = new EmployeeGroupsSimpleResult();
            result.Data.AddRange(groupLst);
            result.Total = dtoLst.Count;
            return result;
        }

        public override async Task<EmployeesSimpleResult> GetSimpleEmployeesByIds(EmployeesByIds request, ServerCallContext context)
        {
            var dtoLst = await employeeApp.GetEmployeeByIds(request.Ids.ToArray());
            var employeeLst = mapper.Map<List<EmployeeSimple>>(dtoLst);
            var result = new EmployeesSimpleResult();
            result.Data.AddRange(employeeLst);
            result.Total = employeeLst.Count;
            return result;
        }

        public override Task<Empty> JoinGroups(GroupAssociationRequest request, ServerCallContext context)
        {
            return base.JoinGroups(request, context);
        }

        public override Task<Empty> LeaveGroups(GroupAssociationRequest request, ServerCallContext context)
        {
            return base.LeaveGroups(request, context);
        }

        public override async Task<EmployeesResult> ListEmployees(ListEmployeesRequest request, ServerCallContext context)
        {
            var pageDto = await employeeApp.SearchEmployees(new EmployeeQueryParameters() { Limit = request.Limit, Offset = request.Offset});
            var employees = mapper.Map<List<Erp.Permissions.Employee>>(pageDto.Data);
            var result = new EmployeesResult();
            result.Total = (int)pageDto.Total;
            result.Data.AddRange(employees);
            return result;
        }

        public override Task<Empty> ResetPasswordByEmail(ResetPasswordByEmailRequest request, ServerCallContext context)
        {
            return base.ResetPasswordByEmail(request, context);
        }

        public override Task<Empty> ResetPasswordByPhoneNumber(ResetPasswordByPhoneNumberRequest request, ServerCallContext context)
        {
            return base.ResetPasswordByPhoneNumber(request, context);
        }

        public override async Task<EmployeesResult> SearchEmployees(SearchEmployeesRequest request, ServerCallContext context)
        {
            var pageDto = await employeeApp.SearchEmployees(new EmployeeQueryParameters() { Limit = request.Limit, Offset = request.Offset , Keyword = request.Keyword, Status = request.Status, RoleId = request.RoleId, GroupId = request.GroupId});
            var result = new EmployeesResult();
            result.Total = (int)pageDto.Total;
            result.Data.AddRange(mapper.Map<List<Erp.Permissions.Employee>>(pageDto.Data));
            return result;
        }

        public override async Task<EmployeesSimpleResult> SearchSimpleEmployees(SearchEmployeesRequest request, ServerCallContext context)
        {
            var pageDto = await employeeApp.SearchEmployees(new EmployeeQueryParameters() { Limit = request.Limit, Offset = request.Offset, Keyword = request.Keyword, Status = request.Status, RoleId = request.RoleId, GroupId = request.GroupId });
            var result = new EmployeesSimpleResult();
            result.Total = (int)pageDto.Total;
            result.Data.AddRange(mapper.Map<List<EmployeeSimple>>(pageDto.Data));
            return result;
        }

        public override async Task<Erp.Permissions.Employee> UpdateEmployee(EmployeeProfile request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
