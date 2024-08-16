using Application.Contracts.IService;
using Domain.Dto;
using Domain.Dto.Employee;
using Domain.Dto.EmployeeRole;
using MiddleWare.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiCore.EmployeeManagement.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("roles")]
    [Produces("application/json")]
    [Tags("角色管理")]
    public class RoleController(IEmployeeRoleAppService roleService,IHttpContextAccessor httpContext) : IdentityBaseController(httpContext)
    {
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<EmployeeRoleDto>),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole([FromBody]CreateEmployeeRoleDto create)
        {
            var newrole = await roleService.CreateRole(create);
            return CreatedAtAction(nameof(CreateRole), newrole);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeRoleDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdateRole(int id,[FromBody] CreateEmployeeRoleDto update)
        {
            update.Id = id;
            var role = await roleService.UpdateRole(update);
            return CreatedAtAction(nameof(CreateRole), role);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<IActionResult> DeleteRole(int id)
        {
            await roleService.DeleteRole(id);
            return NoContent();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<EntitiesResultDto<EmployeeRoleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles([FromQuery] SearchRequestInput search)
        {
            return Ok(await roleService.SearchRoles(search));
        }
        
        /// <summary>
        /// 根据ID获取角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            return Ok(await roleService.GetRoleById(id));
        }

        [HttpPost("{id}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<EntitiesResultDto<bool>>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GrantPermissionsToRole(int id, [FromBody] List<string> permissions)
        {
            return Ok(await roleService.GrantPermissionsToRole(id, permissions));
        }

        [HttpGet("{id}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<EntitiesResultDto<string>>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRolePermissions(int id)
        {
            return Ok(await roleService.GetRolePermissions(id));
        }

        [HttpGet("{id}/employees")]
        [ProducesResponseType(typeof(ApiResponse<EntitiesResultDto<EmployeeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployeesForRole(int id)
        {
            return Ok(await roleService.GetEmployeesForRole(id));
        }
    }
}
