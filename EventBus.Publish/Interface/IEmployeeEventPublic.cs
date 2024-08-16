using Domain.Dto.Employee;
using Domain.Entity;
using Domain.Message;

namespace EventBus.Publish.Interface;

/// <summary>
/// 定义处理员工事件发布的接口。
/// </summary>
public interface IEmployeeEventPublic
{
    /// <summary>
    /// 异步发布员工删除完成的事件。
    /// </summary>
    /// <param name="id">被删除员工的ID。</param>
    /// <param name="@operator">操作者信息。</param>
    /// <returns>一个任务，表示异步操作的完成。</returns>
    Task EmployeeDeleteCompleteAsync(int id, Operator @operator);

    /// <summary>
    /// 发布员工更新完成事件
    /// </summary>
    /// <param name="employeeDto"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    Task EmployeeUpdateCompleteAsync(EmployeeDto employeeDto, Operator @operator);

    /// <summary>
    /// 创建员工完成事件
    /// </summary>
    /// <param name="employeeDto"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    Task EmployeeCreateCompleteAsync(EmployeeDto employeeDto, Operator @operator);
}
