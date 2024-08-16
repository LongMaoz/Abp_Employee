using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Dto.EmployeeGroup;

public class CreateGroupDto
{
    [JsonProperty("name")]
    [Required(ErrorMessage = "员工组名不允许为空")]
    [SwaggerSchema(Description = "员工组名", Nullable = false)]
    public string Name { get; set; }

    [JsonProperty("description")]
    [SwaggerSchema(Description = "组描述", Nullable = true)]
    public string Description { get; set; }
}