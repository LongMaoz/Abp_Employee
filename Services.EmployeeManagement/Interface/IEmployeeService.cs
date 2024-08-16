using Domain.Dto.Employee;
using Domain.Entity;
using Domain.Message;
using Domain.Shared.Enums;
using Volo.Abp.Service;
using Volo.Abp.Domain.Services;

namespace Services.EmployeeManagement.Interface;

public interface IEmployeeService: IDomainService
{
    Task<Employee> CreateEmployeeAsync(CreateEmployeeDto input);

    Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto input);

    Task<Employee> UpdateEmployeeByIdAsync(int id, CreateEmployeeDto input);

    Task<Employee> UpdateEmployeeStatus(int id,EmployeeStatus status);

    Task<Employee> UpdateEmployeePassword(int id, string password);

    Task LeaveGroups(int employeeId, IEnumerable<int> employeeGroupIds);

    Task JoinGroups(int employeeId, List<int> employeeGroupIds);

    Task<bool> DestroyEmployee(int employeeId);

    /// <summary>
    /// 搜索 分页
    /// </summary>
    /// <param name="employeeQueryParameters"></param>
    /// <returns></returns>
    Task<(List<Employee>,int)> SearchEmployees(EmployeeQueryParameters employeeQueryParameters);

    Task<List<Employee>> SearchSimpleEmployeesNoPage(EmployeeQueryAllParameters parameters);

    Task<Employee> GetEmployeeById(int id);

    Task<List<Employee>> GetEmployeeByIds(int[] ids);

    Task<Employee> GetEmployeeByEmail(string email);

    Task<Employee> GetEmployeeByPhoneNumber(string phoneNumber);

    /// <summary>
    /// 获取未分组员工
    /// </summary>
    /// <returns></returns>
    Task<List<Employee>> GetEmployeeNotGroup();

}