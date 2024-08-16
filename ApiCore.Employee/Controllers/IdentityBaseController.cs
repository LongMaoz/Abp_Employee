using Microsoft.AspNetCore.Mvc;

namespace ApiCore.EmployeeManagement.Controllers
{
    public class IdentityBaseController : ControllerBase
    {

        protected ILogger<IdentityBaseController> _logger;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        public string UserName { get; set; }

        public IdentityBaseController(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                this.UserId = httpContextAccessor.HttpContext.User.FindFirst("sub") != null ? Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value) : -1;
                this.UserName = httpContextAccessor.HttpContext.User.FindFirst("name")?.ToString()??"";
            }
        }
    }
}
