using Application.Exception;
using EventBus.Publish;
using Services.EmployeeManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using AliCloud;
using GrpcService;
using Volo.Abp.Application;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Application;

/// <summary>
/// IRentals应用程序模块，用于配置应用程序级服务和映射。
/// </summary>
[DependsOn(
    typeof(GrpcServiceModule),
    typeof(AliCloudModule),
    typeof(EventBusPublishModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(EmployeeManagementServiceModule)
    )]
public class ApplicationModule : AbpModule
{
    /// <summary>
    /// 配置服务方法
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        Configure<AbpAutoMapperOptions>(options =>
        {
            // 添加自动映射配置 用于auto api controller
            options.AddMaps<ApplicationModule>();
        });
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {

        base.PreConfigureServices(context);
    }
}
