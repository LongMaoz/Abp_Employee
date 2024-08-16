using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace CustomEntityFramework;

[DependsOn(typeof(AbpAutoMapperModule),typeof(AbpAutofacModule))]
public class CustomEntityFrameworkModule:AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CustomDbContext>(options =>
        {
            options.AddDefaultRepositories();
        });
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseMySQL();
        });
        return base.ConfigureServicesAsync(context);
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        return base.OnApplicationInitializationAsync(context);
    }
}