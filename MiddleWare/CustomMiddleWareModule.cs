using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MiddleWare;

[DependsOn(typeof(AbpAutofacModule))]
public class CustomMiddleWareModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddTransient<HttpRequestRecordMiddleware>();
        context.Services.AddTransient<HttpResponseMiddleware>();
    }


    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        context.GetApplicationBuilder().UseMiddleware<HttpRequestRecordMiddleware>().UseMiddleware<HttpResponseMiddleware>();
        return base.OnApplicationInitializationAsync(context);
    }
}