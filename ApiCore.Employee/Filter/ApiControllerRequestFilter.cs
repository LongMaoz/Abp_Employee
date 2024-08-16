using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiCore.EmployeeManagement.Filter;

public class ApiControllerRequestFilter: IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        throw new NotImplementedException();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        throw new NotImplementedException();
    }
}