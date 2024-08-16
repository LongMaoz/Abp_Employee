using Domain.Shared.Enums;
using Newtonsoft.Json;
using Volo.Abp.Application.Dtos;

namespace Domain.Dto.Employee;

public class EmployeeBasicDto:EntityDto<int>
{
    public string Avatar { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    [JsonProperty("phoneNumber")]
    public string PhoneNumber { get; set; }
    public EmployeeStatus Status { get; set; }
    [JsonProperty("deleteTime")]
    public DateTime? DeleteTime { get; set; }
}