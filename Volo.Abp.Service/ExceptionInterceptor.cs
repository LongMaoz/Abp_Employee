using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace Volo.Abp.Service;

public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : AbpInterceptor, ITransientDependency
{
    public override async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        if (invocation.Method.IsDefined(typeof(AppExceptionAttribute), true))
        {
            try
            {
                await invocation.ProceedAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"{invocation.Method.Name} {JsonConvert.SerializeObject(invocation.ArgumentsDictionary)},{ex.Message},{ex.InnerException?.Message},{ex.Source},{ex.StackTrace}");
                throw new Exception("An error occurred.", ex);
            }
        }
        else
            await invocation.ProceedAsync();
    }
}