using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace GrpcService
{
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class GrpcServiceModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.OnRegistered(register =>
            {
                if (register.ImplementationType.Name.EndsWith("Grpc"))
                {
                    register.Interceptors.TryAdd<GrpcCacheInterceptor>();
                    register.Interceptors.TryAdd<GrpcClientInterceptor>();
                }
            });
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddScoped<GrpcHelper>();
            context.Services.AddScoped<GrpcClientInterceptor>();
            var types = typeof(GrpcServiceModule).Assembly.GetTypes();
            var interfaces = types.WhereIf(true, c => c.Name.StartsWith("I")).ToList();
            var impls = types.WhereIf(true, c => !c.Name.StartsWith("I")).ToList();
            interfaces.ForEach(item =>
            {
                var imp = impls.Find(c => c.Name.Contains(item.Name.Substring(1)));
                if (imp != null)
                    context.Services.AddScoped(item, imp);
            });
            context.Services.AddAutoMapperObjectMapper<GrpcServiceModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<GrpcServiceModule>(validate: true);
            });
        }
    }
}
