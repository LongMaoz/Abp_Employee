using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Domain.Entity;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using Domain.DataFilter;

namespace EntityFrameworkCore;

/// <summary>
/// 员工管理的数据库上下文类，用于配置和管理数据库中的实体。
/// </summary>
[ConnectionStringName("ErpConnectionString")]
public class EmployeeManagementDbContext : AbpDbContext<EmployeeManagementDbContext>
{
    // 定义数据库中的实体集
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeGroup> EmployeeGroups { get; set; }
    public DbSet<EmployeeInEmployeeGroup> EmployeeInEmployeeGroups { get; set; }
    public DbSet<EmployeeRole> Roles { get; set; }

    protected bool IsCustomSoftDeleteFilterEnabled => DataFilter?.IsEnabled<ICustomSoftDelete>() ?? false;

    /// <summary>
    /// 构造函数，接收数据库上下文选项并传递给基类。
    /// </summary>
    /// <param name="options">数据库上下文选项。</param>
    public EmployeeManagementDbContext(DbContextOptions<EmployeeManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 在模型创建时配置实体映射。
    /// </summary>
    /// <param name="modelBuilder">用于构建模型的ModelBuilder实例。</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 应用实体到数据库表的映射配置
        modelBuilder.Entity<Employee>(EmployeeMapping.MapInfo);
        modelBuilder.Entity<EmployeeGroup>(EmployeeGroupMapping.MapInfo);
        modelBuilder.Entity<EmployeeInEmployeeGroup>(EmployeeInEmployeeGroupMapping.MapInfo);
        modelBuilder.Entity<EmployeeRole>(EmployeeRoleMapping.MapInfo);

        // 为支持自定义软删除的实体应用全局查询过滤器
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ICustomSoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var filter = Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, "DeleteTime"),
                        Expression.Constant(null, typeof(DateTime?))
                    ),
                    parameter
                );
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// 配置数据库上下文的一些选项，例如启用敏感数据日志记录。
    /// </summary>
    /// <param name="optionsBuilder">用于配置上下文选项的OptionsBuilder实例。</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 启用敏感数据日志记录，有助于调试，但在生产环境中应谨慎使用
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// 确定是否应为给定的实体类型应用过滤器。
    /// </summary>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    /// <param name="entityType">实体的元数据。</param>
    /// <returns>如果应为实体应用过滤器，则为true；否则为false。</returns>
    protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
    {
        if (typeof(ICustomSoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            return true;
        }
        return base.ShouldFilterEntity<TEntity>(entityType);
    }

    /// <summary>
    /// 为支持自定义软删除的实体创建过滤表达式。
    /// </summary>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    /// <returns>过滤表达式。</returns>
    protected override Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
    {
        var expression = base.CreateFilterExpression<TEntity>();

        if (typeof(ICustomSoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> isActiveFilter =
                e => !IsCustomSoftDeleteFilterEnabled || EF.Property<DateTime?>(e, "DeleteTime") == null;
            expression = expression == null
                ? isActiveFilter
                : QueryFilterExpressionHelper.CombineExpressions(expression, isActiveFilter);
        }

        return expression;
    }
}
