using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Volo.Abp.Service;

public class AppVoloAbpModule: AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.OnRegistered(register =>
        {
            if (register.ImplementationType.IsDefined(typeof(AppExceptionAttribute), true))
            {
                register.Interceptors.TryAdd<ExceptionInterceptor>();
            }
        });
    }
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddScoped<UnitAppManage, UnitAppManage>();
    }
}