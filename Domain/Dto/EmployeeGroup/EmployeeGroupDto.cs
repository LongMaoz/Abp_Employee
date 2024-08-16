using Domain.Dto.Employee;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Domain.Dto.EmployeeGroup
{
    public class EmployeeGroupDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public int? EmployeesCount { get; set; }
        [JsonIgnore]
        public List<EmployeeBasicDto> EmployeesList { get;set;  }
        public DateTime? CreateTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateTime { get; set; } = DateTime.UtcNow;
        public DateTime? DeleteTime { get; set; }
        public EntitiesResultDto<EmployeeBasicDto> Employees => SimplifiedEmployees;

        [JsonIgnore]
        public EntitiesResultDto<EmployeeBasicDto> SimplifiedEmployees
        {
            get
            {
                if (EmployeesList == null || !EmployeesList.Any())
                {
                    return new EntitiesResultDto<EmployeeBasicDto>
                    {
                        Total = 0,
                        Data = new List<EmployeeBasicDto>()
                    };
                }

                var simplifiedList = EmployeesList.Where(x=>x!= null).Select(employee => new EmployeeBasicDto
                {
                    Name = employee?.Name,
                    Email = employee?.Email,
                    PhoneNumber = employee?.PhoneNumber,
                    Id = employee.Id,
                }).ToList();

                return new EntitiesResultDto<EmployeeBasicDto>
                {
                    Total = EmployeesCount ?? simplifiedList.Count,
                    Data = simplifiedList
                };
            }
        }
    }
}
