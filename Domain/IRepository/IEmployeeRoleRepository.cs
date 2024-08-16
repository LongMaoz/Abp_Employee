using Domain.Entity;
using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;

namespace Domain.IRepository;

public interface IEmployeeRoleRepository:IRepository<EmployeeRole,int>
{

    Task<int> GetCountAsync(Expression<Func<EmployeeRole, bool>> expression);

    Task<bool> SoftDeleteAsync(Expression<Func<EmployeeRole, bool>> expression);
}