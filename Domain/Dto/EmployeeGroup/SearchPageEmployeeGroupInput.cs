using Domain.Shared.Enums;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.EmployeeGroup;

public class SearchPageEmployeeGroupInput: SearchRequestInput
{
    [JsonProperty("returnEmployee")]
    [SwaggerParameter("是否返回员工")]
    public ReturnOption? ReturnEmployee { get; set; }
}