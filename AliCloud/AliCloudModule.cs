
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.MailKit;
using Volo.Abp.Modularity;
using Volo.Abp.Sms.Aliyun;

namespace AliCloud;

[DependsOn(typeof(AbpSmsAliyunModule),typeof(AbpMailKitModule), typeof(AbpDddApplicationModule))]
public class AliCloudModule:AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        IConfiguration configuration = context.Services.GetConfiguration();
        Configure<AbpAliyunSmsOptions>(op =>
        {
            // 阿里云 API 的访问 Id。
            op.AccessKeyId = configuration["AliCloud:Auth:AccessKeyId"];
            // 阿里云 API 的访问密钥。
            op.AccessKeySecret = configuration["AliCloud:Auth:AccessKeySecret"];
        });
    }
}