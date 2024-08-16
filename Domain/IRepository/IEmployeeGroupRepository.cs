using Domain.Entity;
using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;

namespace Domain.IRepository;

public interface IEmployeeGroupRepository : IRepository<EmployeeGroup, int>
{
    Task<bool> SoftDeleteAsync(Expression<Func<EmployeeGroup, bool>> expression);
}