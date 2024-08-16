using Asp.Versioning;
using Application.Contracts.IService;
using Application.Service;
using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeGroup;
using Domain.Entity;
using Domain.Shared.Enums;
using MiddleWare.Model;
using Services.EmployeeManagement.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

namespace ApiCore.EmployeeManagement.Controllers
{
    /// <summary>
    /// 员工管理
    /// </summary>
    /// <param name="employeeAppService"></param>
    [ApiController]
    [Route("employees")]
    [Produces("application/json")]
    [Tags("员工管理")]
    [Authorize]
    public class EmployeeController(IEmployeeAppService employeeAppService,IHttpContextAccessor httpContext) : IdentityBaseController(httpContext)
    {

        [AllowAnonymous]
        [HttpGet("send-message")]
        public async  Task SendMessage()
        {
            await employeeAppService.TestMessageSend("17673631708", "9527");
        }

        [AllowAnonymous]
        [HttpGet("send-email")]
        public async Task SendEmail()
        {
            await employeeAppService.TestEmailSend();
        }

        /// <summary>
        /// 创建员工
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto input)
        {
            // 调用服务层方法创建员工 
            var newemployee = await employeeAppService.CreateEmployee(input,new Domain.Message.Operator() { ID = this.UserId,Name = this.UserName });
            return CreatedAtAction(nameof(CreateEmployee), newemployee);
        }

        /// <summary>
        /// 修改自己的信息
        /// </summary>
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto input)
        {
            await employeeAppService.UpdateEmployee(this.UserId,input);
            return Ok("修改成功");
        }

        /// <summary>
        /// 员工列表
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<EntitiesResultDto<EmployeeDto>>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployees([FromQuery] EmployeeQueryParameters parameters)
        {
            return Ok(await employeeAppService.SearchEmployees(parameters));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto input)
        {
            await employeeAppService.UpdatePassword(this.UserId,input.OldPassword,input.NewPassword);
            return StatusCode(201, "密码修改成功");
        }

        /// <summary>
        /// 根据id修改员工
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] CreateEmployeeDto input)
        {
            return Ok(await employeeAppService.UpdateEmployeeById(id, input, new Domain.Message.Operator() { ID = this.UserId,Name = this.UserName }));
        }

        /// <summary>
        /// 删除员工
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var bl = await employeeAppService.DeleteEmployeeById(id, new Domain.Message.Operator() { ID = this.UserId, Name = this.UserName });
            return bl ? Ok(): NoContent();
        }

        /// <summary>
        /// 获取员工信息
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>),StatusCodes.Status200OK)]

        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await employeeAppService.GetAsync(id);
            return Ok(employee);
        }

        /// <summary>
        /// 获取所有员工(不分页)
        /// </summary>
        [HttpGet("simple/all")]
        [ProducesResponseType(typeof(ApiResponse<List<EmployeeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEmployees([FromQuery] EmployeeQueryAllParameters employeeQueryAllParameters)
        {
            return Ok(await employeeAppService.SearchSimpleEmployeesNoPage(employeeQueryAllParameters));
        }

        /// <summary>
        /// 获取员工分组
        /// </summary>
        [HttpGet("{id}/groups")]
        [ProducesResponseType(typeof(ApiResponse<List<EmployeeGroupDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployeeGroups(int id)
        {
            return Ok(await employeeAppService.GetEmployeeGroups(id));
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        [HttpPost("forget/password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto input)
        {
            await employeeAppService.ForgetPassword(input);
            return NoContent();
        }

        /// <summary>
        /// 发送验证码(忘记密码)
        /// </summary>
        [HttpPost("forgetPassword/sendVerifyCode")]
        public async Task<IActionResult> SendVerifyCode([FromBody] SendVerifyCodeDto input)
        {
            await employeeAppService.SendVerifyCode(input);
            return StatusCode(201, "验证码发送成功");
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Email))
            {
                await employeeAppService.ResetPasswordByEmail(input);
            }
            else if (!string.IsNullOrWhiteSpace(input.PhoneNumber))
            {
                await employeeAppService.ResetPasswordByPhoneNumber(input);
            }
            return StatusCode(201, "密码重置成功");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        [HttpPut("{id}/resetPassword")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            await employeeAppService.ResetPassword(id);
            return StatusCode(201);
        }
        
        /// <summary>
        /// 修改员工状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("{id}/updateStatus/{status}")]
        public async Task<IActionResult> UpdateEmployeeStatus(int id, EmployeeStatus status)
        {
            var bl = await employeeAppService.UpdateStatus(id, status, new Domain.Message.Operator() { ID = this.UserId, Name = this.UserName });
            HttpContext.Items["businessCode"] = 201;
            return bl? Ok("修改成功") : StatusCode(500,"修改失败");
        }
    }
}
