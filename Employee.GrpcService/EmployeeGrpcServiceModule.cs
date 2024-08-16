using Google.Protobuf.WellKnownTypes;
using Application;
using Employee.GrpcService.Profile;
using Employee.GrpcService.Services;
using Volo.Abp.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Employee.GrpcService;

[DependsOn(
    typeof(ApplicationModule),
    typeof(AbpAspNetCoreModule),
    typeof(AppVoloAbpModule),
    typeof(AbpAutofacModule),
    typeof(AbpAutoMapperModule)
)]
public class EmployeeGrpcServiceModule:AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            //Add all mappings defined in the assembly of the MyModule class
            options.AddMaps<GrpcEmployeeProfile>();
        });
        context.Services.AddGrpc();
        return base.ConfigureServicesAsync(context);
    }

    public override Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        return base.OnPreApplicationInitializationAsync(context);
    }

    public override Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        return base.OnPostApplicationInitializationAsync(context);
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        //app.UseRouting();
        //app.UseAuthentication();
        //app.UseConfiguredEndpoints(x =>
        //{
        //    x.MapGrpcService<GrpcEmployeesService>();
        //    x.MapGrpcService<GrpcEmployeeRoleService>();
        //    x.MapGrpcService<GrpcEmployeeGroupsService>();
        //});
        return base.OnApplicationInitializationAsync(context);
    }
}