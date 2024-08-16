 using AutoMapper;
using Feather.Rabbitmq.Register;
using Domain.Dto.Employee;
using Domain.Entity;
using Domain.Message;
using Domain.Message.Employee;
using Domain.Message.EmployeeGroup;
using Domain.Message.EmployeeRole;
using Domain.Shared.Enums;
using Domain.Shared.Extends;
using EventBus.Publish.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace EventBus.Publish.Public;

/// <summary>
/// 员工事件发布类，用于处理员工相关事件的发布。
/// </summary>
public class EmployeeEventPublic : IEmployeeEventPublic
{
    private readonly IBaseMsgPublish _baseMsgPublish;
    private readonly ILogger<EmployeeEventPublic> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数，注入消息发布基础服务和日志服务。
    /// </summary>
    /// <param name="baseMsgPublish">消息发布基础服务。</param>
    /// <param name="logger">日志服务。</param>
    public EmployeeEventPublic(IBaseMsgPublish baseMsgPublish, ILogger<EmployeeEventPublic> logger,IMapper mapper)
    {
        _baseMsgPublish = baseMsgPublish;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// 异步发布员工删除完成的事件。
    /// </summary>
    /// <param name="id">被删除员工的ID。</param>
    /// <param name="operator">操作者信息。</param>
    /// <returns>一个任务，表示异步操作的完成。</returns>
    public async Task EmployeeDeleteCompleteAsync(int id, Operator @operator)
    {
        // 构造删除员工的消息
        var message = new EmployeeMessage.Delete()
        {
            Id = id,
            Operator = @operator,
        };
        // 记录开始发送消息的日志
        _logger.LogInformation($"开始发送删除员工消息 {JsonConvert.SerializeObject(message)}");
        // 使用基础消息发布服务异步发布消息
        await _baseMsgPublish.DynamicMsgPublishAsync(message, RabbitRoutingKeyEnum.EmployeeDeleted.GetString(),
            RabbitExchangeEnum.Employee.GetString(), keyId: id.ToString());
        // 记录发送消息结束的日志
        _logger.LogInformation($"结束发送删除员工消息 {JsonConvert.SerializeObject(message)}");
    }

    /// <summary>
    /// 员工更新事件
    /// </summary>
    /// <param name="employeeDto"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    public async Task EmployeeUpdateCompleteAsync(EmployeeDto employeeDto, Operator @operator)
    {
        var message = new EmployeeMessage.Update()
        {
            Employee = _mapper.Map<EmployeeDto,EmployeeMessageInfo>(employeeDto),
            Roles = employeeDto.RolesList.Select(x=> new EmployeeRoleInfo() { Id = x.Id,Name = x.Name}).ToList(),
            Groups = employeeDto.GroupsList.Select(x=> new EmployeeGroupInfo() { Id = x.Id,Name = x.Name}).ToList(),
            Operator = @operator
        };
        _logger.LogInformation($"开始发送更新员工消息 {JsonConvert.SerializeObject(message)}");
        await _baseMsgPublish.DynamicMsgPublishAsync(message, RabbitRoutingKeyEnum.EmployeeUpdated.GetString(),
            RabbitExchangeEnum.Employee.GetString(), keyId: employeeDto.Id.ToString());
        _logger.LogInformation($"结束发送更新员工消息 {JsonConvert.SerializeObject(message)}");
    }
    
    /// <summary>
    /// 员工创建事件
    /// </summary>
    /// <param name="employeeDto"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    public async Task EmployeeCreateCompleteAsync(EmployeeDto employeeDto, Operator @operator)
    {
        _logger.LogInformation($"开始发送创建员工消息 {JsonConvert.SerializeObject(employeeDto)}");
        var message = new EmployeeMessage.Create()
        {
            Employee = _mapper.Map<EmployeeDto, EmployeeMessageInfo>(employeeDto),
            Roles = employeeDto.RolesList.Select(x => new EmployeeRoleInfo() { Id = x.Id, Name = x.Name }).ToList(),
            Groups = employeeDto.GroupsList.Select(x => new EmployeeGroupInfo() { Id = x.Id, Name = x.Name }).ToList(),
            Operator = @operator,
        };
        await _baseMsgPublish.DynamicMsgPublishAsync(message, RabbitRoutingKeyEnum.EmployeeInserted.GetString(),
            RabbitExchangeEnum.Employee.GetString(), keyId: employeeDto.Id.ToString());
        _logger.LogInformation($"结束发送创建员工消息 {JsonConvert.SerializeObject(employeeDto)}");
    }
}
