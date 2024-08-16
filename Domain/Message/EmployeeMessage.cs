using Domain.Dto.Employee;
using Domain.Entity;
using Domain.Message.Employee;
using Domain.Message.EmployeeGroup;
using Domain.Message.EmployeeRole;
using Newtonsoft.Json;

namespace Domain.Message;

public class EmployeeMessage
{
    
    public class Update()
    {
        [JsonProperty("operator_time")]
        public DateTime OperatorTime { get; set; } = DateTime.UtcNow;

        [JsonProperty("employee")]
        public EmployeeMessageInfo Employee { get; set; }

        [JsonProperty("roles")]
        public List<EmployeeRoleInfo> Roles { get; set; }

        [JsonProperty("groups")]
        public List<EmployeeGroupInfo> Groups { get; set; }

        [JsonProperty("operator")]
        public Operator Operator { get; set; }
    }

    public class Delete()
    {
        public DateTime OperatorTime { get; set; } = DateTime.UtcNow;
        public int Id { get; set; }
        public Operator Operator { get; set; }
    }

    public class Create(): Update
    {

    }
}