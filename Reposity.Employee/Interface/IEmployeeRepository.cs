using IRentals.Entity.EmployeeManagement.Entity;
using Volo.Abp.Domain.Repositories;

namespace IRentals.Repository.EmployeeManagement;

public interface IEmployeeRepository: IRepository<Employee,int>
{
    
}