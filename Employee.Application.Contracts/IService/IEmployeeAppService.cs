using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Message;
using Domain.Shared.Enums;
using Volo.Abp;
using Volo.Abp.Application.Services;
namespace Application.Contracts.IService;

/// <summary>
/// 员工管理
/// </summary>
public interface IEmployeeAppService: ICrudAppService<EmployeeDto,int>, IApplicationService
{
    public Task TestEmailSend();

    public Task TestMessageSend(string phone, string code);

    /// <summary>
    /// 创建员工
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<EmployeeDto> CreateEmployee(CreateEmployeeDto entity, Operator @operator);

    /// <summary>
    /// 修改自己的信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<bool> UpdateEmployee(int id,UpdateEmployeeDto entity);

    /// <summary>
    /// 修改员工信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entity"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    Task<EmployeeDto> UpdateEmployeeById(int id, CreateEmployeeDto entity, Operator @operator);

    /// <summary>
    /// 删除员工
    /// </summary>
    /// <param name="id"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    Task<bool> DeleteEmployeeById(int id,Operator @operator);

    /// <summary>
    /// 员工列表
    /// </summary>
    /// <param name="employeeQueryParameters"></param>
    /// <returns></returns>
    Task<EntitiesResultDto<EmployeeDto>> SearchEmployees(EmployeeQueryParameters employeeQueryParameters);

    /// <summary>
    /// 员工列表(不分页)
    /// </summary>
    /// <param name="employeeQueryAllParameters"></param>
    /// <returns></returns>
    Task<List<EmployeeDto>> SearchSimpleEmployeesNoPage(EmployeeQueryAllParameters employeeQueryAllParameters);

    /// <summary>
    /// 获取员工分组
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<EmployeeGroupBasicDto>> GetEmployeeGroups(int id);


    /// <summary>
    /// 获取员工信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<EmployeeDto> GetEmployeeById(int id);

    Task<List<EmployeeDto>> GetEmployeeByIds(int[] ids);


    /// <summary>
    /// 忘记密码
    /// </summary>
    /// <param name="forgetPassword"></param>
    /// <returns></returns>
    Task ForgetPassword(ResetPasswordDto forgetPassword);

    /// <summary>
    /// 忘记密码（邮箱）
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task ForgetPasswordByEmail(string email);

    Task ForgetPasswordByPhoneNumber(string phoneNumber);

    /// <summary>
    /// 发送验证码(忘记密码)
    /// </summary>
    /// <param name="forgetPassword"></param>
    /// <returns></returns>
    Task<bool> SendVerifyCode(SendVerifyCodeDto forgetPassword);

    /// <summary>
    /// 重置密码(忘记密码)
    /// </summary>
    /// <param name="resetPassword"></param>
    /// <returns></returns>
    Task<bool> ResetPasswordByForget(ResetPasswordByForgetDto resetPassword);

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> ResetPassword(int id);

    Task ResetPasswordByEmail(ForgetPasswordDto forgetPassword);

    Task ResetPasswordByPhoneNumber(ForgetPasswordDto forgetPassword);

    Task UpdatePassword(int id,string oldPwd, string newPwd);

    /// <summary>
    /// 修改员工状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<bool> UpdateStatus(int id, EmployeeStatus status, Operator @operator);
}