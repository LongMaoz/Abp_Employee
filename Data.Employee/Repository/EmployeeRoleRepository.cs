using System.Linq.Expressions;
using Domain.Entity;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EntityFrameworkCore.Repository;

public class EmployeeRoleRepository(IDbContextProvider<EmployeeManagementDbContext> dbContextProvider)
    : EfCoreRepository<EmployeeManagementDbContext, EmployeeRole, int>(dbContextProvider), IEmployeeRoleRepository
{

    public async Task<int> GetCountAsync(Expression<Func<EmployeeRole, bool>> expression)
    {
        var dbcontext = await this.GetDbContextAsync();
        return await dbcontext.Roles.Where(expression).CountAsync();
    }

    public async Task<bool> SoftDeleteAsync(Expression<Func<EmployeeRole, bool>> expression)
    {
        var dbcontext = await this.GetDbContextAsync();
        return await dbcontext.Roles.Where(expression).ExecuteUpdateAsync(x => x.SetProperty(z=>z.DeleteTime,DateTime.Now)) > 0;
    }
}