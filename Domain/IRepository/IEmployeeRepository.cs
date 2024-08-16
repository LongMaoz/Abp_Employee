using Domain.Entity;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Domain.IRepository;

public interface IEmployeeRepository : IEfCoreRepository<Employee, int>, IRepository<Employee, int>
{
    Task<bool> UpdateBulkAsync(Expression<Func<Employee, bool>> whereExpression, Expression<Func<SetPropertyCalls<Employee>, SetPropertyCalls<Employee>>> updateExpression);

    Task<bool> SoftDeleteAsync(Expression<Func<Employee, bool>> expression);
}