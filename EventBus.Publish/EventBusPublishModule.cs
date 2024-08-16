using Feather.Rabbitmq.Register;
using EventBus.Publish.Interface;
using EventBus.Publish.Public;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace EventBus.Publish;

/// <summary>
/// EventBusPublish模块，用于配置和初始化事件总线发布相关服务。
/// </summary>
[DependsOn()]
public class EventBusPublishModule : AbpModule
{
    /// <summary>
    /// 在服务注册阶段配置模块的服务。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        // 注册事件发布服务
        context.Services.AddScoped<IEmployeeEventPublic, EmployeeEventPublic>();
        // 初始化RabbitMQ服务
        context.Services.InitRabbitMQService(context.Services.GetConfiguration());
    }

    /// <summary>
    /// 应用程序初始化时调用。可以在此添加自定义的初始化代码。
    /// </summary>
    /// <param name="context">应用程序初始化上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        return base.OnApplicationInitializationAsync(context);
    }

    /// <summary>
    /// 应用程序初始化之前调用。可以在此添加自定义的预初始化代码。
    /// </summary>
    /// <param name="context">应用程序初始化上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public override Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // 使用RabbitMQ服务的消费者-置空为null
        context.GetApplicationBuilder().UseConsumerRabbitMQService(null);
        return base.OnPreApplicationInitializationAsync(context);
    }
}
