using CustomEntityFramework;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Api.Employee;

[DependsOn(typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreModule),
    typeof(AbpAutoMapperModule),
    typeof(CustomEntityFrameworkModule))]
public class ApiEmployeeModule:AbpModule
{
    
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddControllers();
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddSwaggerGen();
        var configuration = context.Services.GetConfiguration();
        this.ConfigSwaggerOptions(context,configuration);
        return base.ConfigureServicesAsync(context);
    }

    public void ConfigSwaggerOptions(ServiceConfigurationContext context,IConfiguration configuration)
    {
        context.Services.Configure<SwaggerGenOptions>(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Employee API", Version = "v1" });
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Api.Employee.xml"));
        });
    }
    
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(x =>
        {
            x.MapControllers();
        });
        return base.OnApplicationInitializationAsync(context);
    }
}