using System.Linq.Expressions;
using Domain.Entity;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EntityFrameworkCore.Repository;

public class EmployeeGroupRepository(IDbContextProvider<EmployeeManagementDbContext> dbContextProvider) 
    : EfCoreRepository<EmployeeManagementDbContext,EmployeeGroup,int>(dbContextProvider),IEmployeeGroupRepository
{
    public async Task<bool> SoftDeleteAsync(Expression<Func<EmployeeGroup, bool>> expression)
    {
        var dbcontext = await this.GetDbContextAsync();
        return await dbcontext.EmployeeGroups.Where(expression).ExecuteUpdateAsync(x => x.SetProperty(z => z.DeleteTime, DateTime.Now)) > 0;
    }
}