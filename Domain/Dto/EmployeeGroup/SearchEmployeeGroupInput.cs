using Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Runtime;
using Newtonsoft.Json;

namespace Domain.Dto.EmployeeGroup
{
    public class GetAllEmployeeGroupInput
    {
        [JsonProperty("returnUngrouped")]
        [SwaggerParameter("是否返回未分组员工")]
        public ReturnOption? ReturnUngrouped { get; set; }

        [JsonProperty("returnEmployee")]
        [SwaggerParameter("是否返回员工")]
        public ReturnOption? ReturnEmployee { get; set; }

        [JsonProperty("status")]
        [SwaggerParameter("员工状态，enabled(正常),disabled(禁用)")]
        public EmployeeStatus? Status { get; set; }
    }
}
