using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Casbin.Persist.Adapter.EFCore;
using Casbin;
using Microsoft.Extensions.Configuration;
using Volo.Abp.EntityFrameworkCore.MySQL;

namespace Casbin
{
    [DependsOn(typeof(AbpEntityFrameworkCoreMySQLModule))]
    public class CasbinModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            string? connectionString = context.Services.GetConfiguration().GetConnectionString("ErpConnectionString");
            context.Services.AddDbContext<CasbinDbContext<int>>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            context.Services.AddSingleton<IEnforcer, Enforcer>(x =>
            {
                var cabinContext = context.Services.GetRequiredService<CasbinDbContext<int>>();
                var efCoreAdapter = new EFCoreAdapter<int>(cabinContext);
                var e = new Enforcer( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "examples/rbac_model.conf"), efCoreAdapter);
                LoadPolicyAsync(e).GetAwaiter().GetResult();
                return e;
            });
            base.ConfigureServices(context);
        }

        private async Task LoadPolicyAsync(Enforcer enforcer)
        {
            await enforcer.LoadPolicyAsync();
        }
    }
}