using System.Security.Cryptography;
using Casbin;
using AliCloud;
using Application.Contracts.IService;
using Config;
using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Dto.EmployeeRole;
using Domain.Entity;
using Domain.IRepository;
using Domain.Message;
using Domain.Shared.Enums;
using Domain.Shared.Extends;
using EventBus.Publish.Interface;
using GrpcService.IGrpc;
using Services.EmployeeManagement.Interface;
using Volo.Abp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUglify;
using OtpNet;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;

namespace Application.Service;

public class EmployeeAppService(
    IAliCloudSmsService smsService,
    IEmployeeService employeeService,
    IEmployeeRoleAppService employeeRoleAppService,
    IEmployeeRoleService employeeRoleService,
    IEmployeeRepository employeeRepository,
    IEmployeeEventPublic employeeEvent,
    IEmailGrpc emailGrpc,
    ILogger<EmployeeAppService> logger,
    IEnforcer enforcer,
    UnitAppManage unitAppManage
    )
    : CrudAppService<Employee, EmployeeDto, int>(employeeRepository), IEmployeeAppService, IValidationEnabled
{

    private bool VerifyVerificationCode(string secret, string code)
    {
        byte[] base32Bytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(base32Bytes, 60);
        bool isValid = totp.VerifyTotp(code, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        return isValid;
    }

    private string GenerateVerificationCode(string secret)
    {
        byte[] base32Bytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(base32Bytes, 60);
        var result = totp.ComputeTotp();
        return result;
    }

    public async Task<bool> UpdateEmployee(int id, UpdateEmployeeDto entity)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(async () =>
        {
            var employee = await employeeService.UpdateEmployeeAsync(id, entity);
            return employee;
        });
        var employee = unitResult.Value;
        EmployeeDto employeeDto = new EmployeeDto();
        ObjectMapper.Map(employee, employeeDto);
        // 发送同步消息
        await employeeEvent.EmployeeUpdateCompleteAsync(employeeDto, new Operator()
        {
            ID = employee.Id,
            Name = employee.Name
        });
        return true;
    }

    public async Task<EmployeeDto> UpdateEmployeeById(int id, CreateEmployeeDto entity, Operator @operator)
    {
        var employee = await employeeService.UpdateEmployeeByIdAsync(id, entity);
        var unitResult = await unitAppManage.TranUnitOfWork(async () =>
        {
            //重置关联角色
            await employeeRoleService.GrantRolesToEmployee(id, entity.RoleIds);

            //删除旧分组关联
            var oldGroups = await this.GetEmployeeGroups(id);
            logger.LogDebug(JsonConvert.SerializeObject(oldGroups));
            if (oldGroups is { Count: > 0 })
            {
                var oldGroupIds = oldGroups.Select(g => g.Id).ToList();
                await employeeService.LeaveGroups(id, oldGroupIds);
            }
            //关联新分组
            if (entity.GroupIds is { Length: > 0 })
            {
                await employeeService.JoinGroups(id, entity.GroupIds.ToList());
            }
            return true;
        });

        EmployeeDto employeeDto = new EmployeeDto();
        ObjectMapper.Map(employee, employeeDto);
        // 发送同步消息
        await employeeEvent.EmployeeUpdateCompleteAsync(employeeDto, @operator);
        return employeeDto;
    }

    public async Task<bool> DeleteEmployeeById(int id, Operator @operator)
    {
        var unitResult = await unitAppManage.TranUnitOfWork(async () => await employeeService.DestroyEmployee(id));
        //发送mq消息
        await employeeEvent.EmployeeDeleteCompleteAsync(id, @operator);
        return unitResult.Result;
    }

    public async Task<EntitiesResultDto<EmployeeDto>> SearchEmployees(EmployeeQueryParameters employeeQueryParameters)
    {
        var result = await employeeService.SearchEmployees(employeeQueryParameters);
        return new EntitiesResultDto<EmployeeDto>()
        {
            Total = result.Item2,
            Data = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(result.Item1)
        };
    }

    public async Task<List<EmployeeDto>> SearchSimpleEmployeesNoPage(EmployeeQueryAllParameters employeeQueryAllParameters)
    {
        var result = await employeeService.SearchSimpleEmployeesNoPage(employeeQueryAllParameters);
        return ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(result);
    }

    public async Task<List<EmployeeGroupBasicDto>> GetEmployeeGroups(int id)
    {
        var employee = await this.GetAsync(id);
        return employee.GroupsList;
    }

    public async Task<EmployeeDto> GetEmployeeById(int id)
    {
        var employeedto = await this.GetAsync(id);
        var roles = await employeeRoleAppService.GetRolesForEmployee(employeedto.Id);
        employeedto.RolesList = roles;
        employeedto.RolesCount = roles.Count;
        return employeedto;
    }

    public async Task<List<EmployeeDto>> GetEmployeeByIds(int[] ids)
    {
        var employeeLst = await employeeService.GetEmployeeByIds(ids);
        var dtoLst = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employeeLst);
        foreach (var item in dtoLst)
        {
            var roles = await employeeRoleAppService.GetRolesForEmployee(item.Id);
            item.RolesList = roles;
            item.RolesCount = roles.Count;
        }
        return dtoLst;
    }

    public async Task ForgetPassword(ResetPasswordDto forgetPassword)
    {
        if (!string.IsNullOrWhiteSpace(forgetPassword.Email))
        {
            await this.ForgetPasswordByEmail(forgetPassword.Email);
        }
        else if (!string.IsNullOrWhiteSpace(forgetPassword.PhoneNumber))
        {
            await this.ForgetPasswordByPhoneNumber(forgetPassword.PhoneNumber);
        }
    }

    public async Task ForgetPasswordByEmail(string email)
    {
        var employee = await employeeService.GetEmployeeByEmail(email);
        var verificationCode = GenerateVerificationCode(employee.Email);
        await emailGrpc.SendSimpleMail(new GrpcService.Model.EmailRequestSimpleModel()
        {
            HtmlBody = $"您的邮件验证码为{verificationCode}，此邮件验证码将用于验证身份，修改密码等，请勿将验证码透漏给其他人。本邮件由系统自动发送，请勿直接回复！",
            Subject = "****验证码",
            ToAddress = email,
        });
        logger.LogInformation($"{email}验证码:{verificationCode}");
    }

    public async Task ForgetPasswordByPhoneNumber(string phoneNumber)
    {
        var employee = await employeeService.GetEmployeeByPhoneNumber(phoneNumber);
        var verificationCode = GenerateVerificationCode(employee.PhoneNumber);
        await smsService.SendCodeAsync(phoneNumber, verificationCode);
        logger.LogInformation($"{employee.PhoneNumber}验证码:{verificationCode}");
    }

    public async Task<bool> SendVerifyCode(SendVerifyCodeDto forgetPassword)
    {
        if (!string.IsNullOrWhiteSpace(forgetPassword.Email))
        {
            await this.ForgetPasswordByEmail(forgetPassword.Email);
        }
        else if (!string.IsNullOrWhiteSpace(forgetPassword.PhoneNumber))
        {
            await this.ForgetPasswordByPhoneNumber(forgetPassword.PhoneNumber);
        }
        return true;
    }

    public async Task<bool> ResetPasswordByForget(ResetPasswordByForgetDto resetPassword)
    {
        if (!string.IsNullOrWhiteSpace(resetPassword.Email))
        {
            if (VerifyVerificationCode(resetPassword.Email, resetPassword.VerificationCode))
            {
                await this.ResetPasswordByEmail(new ForgetPasswordDto()
                {
                    Email = resetPassword.Email,
                    VerificationCode = resetPassword.VerificationCode,
                    Password = resetPassword.Password
                });
            }
            else
            {
                throw new BusinessException("500", "提供的验证码无效。");
            }
        }
        else if (!string.IsNullOrWhiteSpace(resetPassword.PhoneNumber))
        {

            if (VerifyVerificationCode(resetPassword.Email, resetPassword.VerificationCode))
            {
                await this.ResetPasswordByPhoneNumber(new ForgetPasswordDto()
                {
                    Email = resetPassword.Email,
                    VerificationCode = resetPassword.VerificationCode,
                    Password = resetPassword.Password
                });
            }
            else
            {
                throw new BusinessException("500", "提供的验证码无效。");
            }
        }
        return true;
    }

    public async Task<bool> ResetPassword(int id)
    {
        var employee = await employeeService.GetEmployeeById(id);
        // 随机生成初始密码
        string initialPassword = GenerateRandomPassword();
        //获取当前运行环境
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "dev")
        {
            initialPassword = "123456";// 非生产环境默认123456
        }
        var hashPwd = PasswordHelper.HashPassword(initialPassword);
        await employeeService.UpdateEmployeePassword(id, hashPwd);
        await emailGrpc.SendSimpleMail(new GrpcService.Model.EmailRequestSimpleModel()
        {
            HtmlBody = $"您的ERP账号重置密码为{initialPassword}。为保护您的账号安全，请尽快登录后重新修改密码。",
            Subject = "账户密码已重置",
            ToAddress = employee.Email,
        });
        return true;
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var password = new char[6];
        using (var rng = RandomNumberGenerator.Create())
        {
            var data = new byte[6];
            rng.GetBytes(data);
            for (int i = 0; i < password.Length; i++)
            {
                var index = data[i] % chars.Length;
                password[i] = chars[index];
            }
        }
        return new string(password);

    }

    public async Task ResetPasswordByEmail(ForgetPasswordDto forgetPassword)
    {
        var employee = await employeeService.GetEmployeeByEmail(forgetPassword.Email);
        // 随机生成初始密码
        string initialPassword = GenerateRandomPassword();
        //获取当前运行环境
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "dev")
        {
            initialPassword = "123456";// 非生产环境默认123456
        }
        var hashPwd = PasswordHelper.HashPassword(initialPassword);
        await employeeService.UpdateEmployeePassword(employee.Id, hashPwd);
    }

    public async Task ResetPasswordByPhoneNumber(ForgetPasswordDto forgetPassword)
    {
        var employee = await employeeService.GetEmployeeByPhoneNumber(forgetPassword.PhoneNumber);
        // 随机生成初始密码
        string initialPassword = GenerateRandomPassword();
        //获取当前运行环境
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "dev")
        {
            initialPassword = "123456";// 非生产环境默认123456
        }
        var hashPwd = PasswordHelper.HashPassword(initialPassword);
        await employeeService.UpdateEmployeePassword(employee.Id, hashPwd);
    }

    public async Task UpdatePassword(int id, string oldPwd, string newPwd)
    {
        var employee = await employeeService.GetEmployeeById(id);
        var verifyResult = PasswordHelper.VerifyHashedPassword(employee.Password, oldPwd);
        if (verifyResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
        {
            var hashPwd = PasswordHelper.HashPassword(newPwd);
            await employeeService.UpdateEmployeePassword(id, hashPwd);
        }
        else
        {
            throw new BusinessException("500", "密码验证失败，请确认密码是否正确。");
        }
        return;
    }

    public async Task<bool> UpdateStatus(int id, EmployeeStatus status, Operator @operator)
    {
        this.Logger.LogInformation($"UpdateStatus: id={id}, status={status.GetString()}");
        var employee = await employeeService.UpdateEmployeeStatus(id, status);
        EmployeeDto dto = new EmployeeDto();
        ObjectMapper.Map(employee, dto);
        //消息同步
        await employeeEvent.EmployeeUpdateCompleteAsync(dto, @operator);
        return true;
    }

    public async Task TestEmailSend()
    {
        await emailGrpc.SendSimpleMail(new GrpcService.Model.EmailRequestSimpleModel()
        {
            Subject = "****验证码",
            HtmlBody = "您的账号初始密码为123456",
            ToAddress = "287175199@qq.com",
        });
    }

    public async Task TestMessageSend(string phone, string code)
    {
        await smsService.SendCodeAsync(phone, code);
    }

    public async Task<EmployeeDto> CreateEmployee(CreateEmployeeDto entity, Operator @operator)
    {
        var hashpwd = PasswordHelper.HashPassword("123456");
        entity.Password = hashpwd;
        var unitResult = await unitAppManage.TranUnitOfWork(async () =>
        {
            //创建员工
            var create = await employeeService.CreateEmployeeAsync(entity);
            //关联角色
            if (entity.RoleIds is { Length: > 0 })
            {
                await employeeRoleService.GrantRolesToEmployee(create.Id, entity.RoleIds);
            }
            //关联分组
            if (entity.GroupIds is { Length: > 0 })
            {
                await employeeService.JoinGroups(create.Id, entity.GroupIds.ToList());
            }
            return ObjectMapper.Map<Employee, EmployeeDto>(create);
        });
        //发送同步消息
        await employeeEvent.EmployeeCreateCompleteAsync(unitResult.Value, @operator);
        return unitResult.Value;
    }
}