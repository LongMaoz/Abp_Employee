using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace EntityFrameworkCore;

/// <summary>
/// 员工管理数据模块，用于配置员工管理领域的数据存储和访问。
/// </summary>
[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpEntityFrameworkCoreMySQLModule)
    )]
public class EmployeeManagementDataModule : AbpModule
{
    /// <summary>
    /// 配置服务方法，在这里注册依赖项和数据库上下文。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);

        // 添加员工管理数据库上下文，并配置其为默认仓储
        context.Services.AddAbpDbContext<EmployeeManagementDbContext>(options =>
        {
            // 包括所有实体到默认仓储
            options.AddDefaultRepositories(includeAllEntities: true);

        });

        // 配置使用MySQL作为数据库提供程序
        Configure<AbpDbContextOptions>(options =>
        {

            //lazy loading
            options.PreConfigure<EmployeeManagementDbContext>(opts =>
            {
                opts.DbContextOptions.UseLazyLoadingProxies();
                opts.DbContextOptions.LogTo(Console.WriteLine, LogLevel.Information);
            });
            options.UseMySQL();
        });
    }
}
