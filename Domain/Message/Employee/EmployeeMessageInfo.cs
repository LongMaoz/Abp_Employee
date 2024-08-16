using Domain.Shared.Enums;
using Newtonsoft.Json;

namespace Domain.Message.Employee;

public class EmployeeMessageInfo: MessageBase
{
    [JsonProperty("display_name")]
    public string DisplayName { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }
    [JsonProperty("password")]
    public string Avatar { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("default_warehouse_id")]
    public int DefaultWarehouseId { get; set; }
}