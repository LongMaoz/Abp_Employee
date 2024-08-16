using Application.Contracts.IService;
using Domain.Dto;
using Domain.Dto.EmployeeGroup;
using Domain.Entity;
using MiddleWare.Model;
using Services.EmployeeManagement.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Domain.Dto.Employee;
using Microsoft.AspNetCore.Authorization;

namespace ApiCore.EmployeeManagement.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("employee-groups")]
    [Produces("application/json")]
    [Tags("员工组管理")]
    [Authorize]
    public class EmployeeGroupsController(IEmployeeGroupAppService groupService,IHttpContextAccessor httpContext) : IdentityBaseController(httpContext)
    {
        /// <summary>
        /// 创建员工组
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK,type: typeof(ApiResponse<EmployeeGroupDto>))]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto group)
        {
            var result = await groupService.CreateGroup(group);
            HttpContext.Items["businessCode"] = 201;
            return Ok(result);
        }

        /// <summary>
        /// 更新员工组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeGroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] CreateGroupDto dto)
        {
            return Ok(await groupService.UpdateGroup(id, dto));
        }

        /// <summary>
        /// 删除员工组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DestroyGroup(int id)
        {
            await groupService.DestroyGroup(id);
            return NoContent();
        }

        /// <summary>
        /// 分页获取所有员工组以及员工
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<EntitiesResultDto<EmployeeGroupDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchGroups([FromQuery] SearchPageEmployeeGroupInput req)
        {
            return Ok(await groupService.SearchGroups(req));
        }

        /// <summary>
        /// 获取所有员工组以及对应员工
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ApiResponse<List<EmployeeGroupDto>>))]
        public async Task<IActionResult> GetAllGroups([FromQuery] GetAllEmployeeGroupInput req)
        {
            return Ok(await groupService.GetGroupsAndEmployees(req));
        }

        /// <summary>
        /// 根据Id获取员工组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ApiResponse<EmployeeGroupDto>))]
        public async Task<IActionResult> GetGroupById(int id)
        {
            return Ok(await groupService.GetAsync(id));
        }

        /// <summary>
        /// 根据组id获取员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/employees")]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ApiResponse<EntitiesResultDto<List<EmployeeDto>>>))]
        public async Task<IActionResult> GetEmployeesOfGroup(int id)
        {
            return Ok(await groupService.GetEmployeesOfGroup(id));
        }

        /// <summary>
        /// 分页根据组id获取员工
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("{id}/employees/page")]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ApiResponse<EntitiesResultDto<List<EmployeeDto>>>))]
        public async Task<IActionResult> GetEmployeesOfGroupByPage(int id, [FromQuery] SearchRequestInput query)
        {
            return Ok(await groupService.GetEmployeesOfGroupByPage(id,query));
        }

        /// <summary>
        /// 往员工组内加入员工
        /// </summary>
        /// <param name="id">分组Id</param>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        [HttpPost("{id}/Employees/{employeeId}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(ApiResponse<string>))]
        public async Task<IActionResult> AddEmployeeToGroup(int id, int employeeId)
        {
            await groupService.AddEmployeeFromGroup([id], employeeId);
            HttpContext.Items["businessCode"] = 201;
            return Ok("新增成功");
        }
        
        /// <summary>
        /// 从员工组内删除员工
        /// </summary>
        /// <param name="id">分组Id</param>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        [HttpDelete("{id}/Employees/{employeeId}")]
        [SwaggerResponse(StatusCodes.Status200OK,type:typeof(ApiResponse<string>))]
        public async Task<IActionResult> DeleteEmployeeFromGroup(int id, int employeeId)
        {
            await groupService.DeleteEmployeeFromGroup([id], employeeId);
            HttpContext.Items["businessCode"] = 204;
            return Ok("删除成功");
        }
    }

}
