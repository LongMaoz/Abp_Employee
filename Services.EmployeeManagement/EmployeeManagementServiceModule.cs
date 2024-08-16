using Casbin;
using EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Services.EmployeeManagement;

[DependsOn(
    typeof(CasbinModule),
    typeof(EmployeeManagementDataModule)
    )]
public class EmployeeManagementServiceModule:AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        base.PreConfigureServices(context);
    }
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}