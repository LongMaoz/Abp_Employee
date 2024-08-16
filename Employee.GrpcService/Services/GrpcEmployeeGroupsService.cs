using AutoMapper;
using Erp.Permissions;
using Grpc.Core;
using Application.Contracts.IService;

namespace Employee.GrpcService.Services;

public class GrpcEmployeeGroupsService(IEmployeeGroupAppService groupAppService,IMapper mapper): EmployeeGroupsService.EmployeeGroupsServiceBase
{
    public override async Task<EmployeeGroupsResult> GetAllGroups(EmptyRequest request, ServerCallContext context)
    {
        var groupLst = await groupAppService.GetAllGroups();
        var result = new EmployeeGroupsResult();
        result.Total = groupLst.Count;
        result.Data.AddRange(mapper.Map<List<EmployeeGroup>>(groupLst));
        return result;
    }

    public override async Task<EmployeeGroup> GetGroupById(EmployeeGroupById request, ServerCallContext context)
    {
        var dto = await groupAppService.GetAsync(request.Id);
        return mapper.Map<EmployeeGroup>(dto);
    }
    
    public override async Task<EmployeeGroupsResult> GetGroupsByIds(EmployeeGroupsByIds request, ServerCallContext context)
    {
        var groupLst = await groupAppService.GetGroupsByIds(request.Ids.ToArray());
        var result = new EmployeeGroupsResult();
        result.Total = groupLst.Count;
        result.Data.AddRange(mapper.Map<List<EmployeeGroup>>(groupLst));
        return result;
    }

    public override async Task<EmployeeGroupsSimpleResult> GetSimpleGroupsByIds(EmployeeGroupsByIds request, ServerCallContext context)
    {
        var groupLst = await groupAppService.GetSimpleGroupsByIds(request.Ids.ToArray());
        var result = new EmployeeGroupsSimpleResult();
        result.Total = groupLst.Count;
        result.Data.AddRange(mapper.Map<List<EmployeeGroupSimple>>(groupLst));
        return result;
    }

    public override async Task<EmployeeGroupsResult> ListGroups(ListEmployeeGroupsRequest request, ServerCallContext context)
    {
        var pageDto = await groupAppService.SearchGroups(new Domain.Dto.EmployeeGroup.SearchPageEmployeeGroupInput() { ReturnEmployee =  Domain.Shared.Enums.ReturnOption.Yes, Limit = request.Limit, Offset = request.Offset,});
        var result = new EmployeeGroupsResult();
        result.Total = (int)pageDto.Total;
        result.Data.AddRange(mapper.Map<List<EmployeeGroup>>(pageDto.Data));
        return result;
    }
    
    public override async Task<EmployeeGroupsResult> SearchGroups(SearchEmployeeGroupsRequest request, ServerCallContext context)
    {
        var pageDto = await groupAppService.SearchGroups(new Domain.Dto.EmployeeGroup.SearchPageEmployeeGroupInput() { ReturnEmployee =  Domain.Shared.Enums.ReturnOption.Yes, Keyword = request.Keyword , Limit = request.Limit, Offset = request.Offset,});
        var result = new EmployeeGroupsResult();
        result.Total = (int)pageDto.Total;
        result.Data.AddRange(mapper.Map<List<EmployeeGroup>>(pageDto.Data));
        return result;
    }
}