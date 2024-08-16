
using Domain.Entity;
using Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.Domain.Entities;

namespace EntityFrameworkCore;

public class BaseMapping
{
    public static void MapInfo<T>(EntityTypeBuilder<T> builder) where T : class, IEntity
    {
        var tableName = builder.Metadata.GetTableName();
        builder.ToTable(tableName);
    }
}
public sealed class EmployeeMapping
{
    public static void MapInfo(EntityTypeBuilder<Employee> builder)
    {
        BaseMapping.MapInfo(builder);
        builder
            .Ignore(e => e.GroupsList)
            .Ignore(e=>e.RolesList)
            .Property(e => e.Status)
            .HasConversion(v => v.ToString(),
            v => (EmployeeStatus)Enum.Parse(typeof(EmployeeStatus), v, true));
    }
}

public sealed class EmployeeGroupMapping
{
    public static void MapInfo(EntityTypeBuilder<EmployeeGroup> builder)
    {
        BaseMapping.MapInfo(builder); 
    }
}

public sealed class EmployeeInEmployeeGroupMapping
{
    public static void MapInfo(EntityTypeBuilder<EmployeeInEmployeeGroup> builder)
    {
        BaseMapping.MapInfo(builder);
        builder
            .HasKey(eig => new { eig.EmployeeId, eig.EmployeeGroupId }); // 设置复合主键

        builder
            .HasOne(eig => eig.Employee)
            .WithMany(e => e.EmployeeInGroups)
            .HasForeignKey(eig => eig.EmployeeId);

        builder
            .HasOne(eig => eig.EmployeeGroup)
            .WithMany(eg => eg.EmployeeInGroups)
            .HasForeignKey(eig => eig.EmployeeGroupId);

        //builder.HasNoKey();
    }
}

public sealed class EmployeeRoleMapping
{
    public static void MapInfo(EntityTypeBuilder<EmployeeRole> builder)
    {
        BaseMapping.MapInfo(builder);
    }
}