using Domain.Entity;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using NUglify;
using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;


namespace EntityFrameworkCore.Repository;

public class EmployeeRepository(IDbContextProvider<EmployeeManagementDbContext> dbContextProvider)
    : EfCoreRepository<EmployeeManagementDbContext, Employee, int>(dbContextProvider), IEmployeeRepository
{
    public async Task<bool> UpdateBulkAsync(Expression<Func<Employee, bool>> whereExpression, Expression<Func<SetPropertyCalls<Employee>, SetPropertyCalls<Employee>>> updateExpression)
    {
        var context = await this.GetDbContextAsync();
        var result = await context.Employees
        .Where(whereExpression)
        .ExecuteUpdateAsync(updateExpression);
        return result > 0;
    }

    public async Task<bool> SoftDeleteAsync(Expression<Func<Employee, bool>> expression)
    {
        var dbcontext = await this.GetDbContextAsync();
        return await dbcontext.Employees.Where(expression).ExecuteUpdateAsync(x => x.SetProperty(z => z.DeleteTime, DateTime.Now)) > 0;
    }

}