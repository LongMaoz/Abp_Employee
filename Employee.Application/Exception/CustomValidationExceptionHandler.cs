using System.Net;
using System.Reflection;
using Domain.Shared.Extends;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http;
using Volo.Abp.Validation;

namespace Application.Exception;

public class CustomValidationExceptionHandler: AbpExceptionFilter
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {

        return base.OnExceptionAsync(context);
    }

    protected override async Task HandleAndWrapException(ExceptionContext context)
    {
        if (context.Exception is AbpValidationException)
        {
            LogException(context, out var remoteServiceErrorInfo);

            await context.GetRequiredService<IExceptionNotifier>().NotifyAsync(new ExceptionNotificationContext(context.Exception));
            context.HttpContext.Response.Headers.Add(AbpHttpConstsExtend.AbpValidationErrorFormat, "true");
            remoteServiceErrorInfo.Code = ((int)HttpStatusCode.BadRequest).ToString();
            context.Result = new ObjectResult(new RemoteServiceErrorResponse(remoteServiceErrorInfo));
            context.ExceptionHandled = true; //Handled!
        }
        else if (context.Exception is BusinessException)
        {
            LogException(context, out var remoteServiceErrorInfo);

            await context.GetRequiredService<IExceptionNotifier>().NotifyAsync(new ExceptionNotificationContext(context.Exception));
            context.HttpContext.Response.Headers.Add(AbpHttpConsts.AbpErrorFormat, "true");
            remoteServiceErrorInfo.Message = context.Exception.Message;
            context.Result = new ObjectResult(new RemoteServiceErrorResponse(remoteServiceErrorInfo));
            context.ExceptionHandled = true; //Handled!
        }
        else
        {
            await base.HandleAndWrapException(context);
        }
    }
}