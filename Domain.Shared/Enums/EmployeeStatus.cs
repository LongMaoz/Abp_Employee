using System.ComponentModel;
using System.Text.Json.Serialization;
using Domain.Shared.Attributes;
using Newtonsoft.Json;

namespace Domain.Shared.Enums;

public enum EmployeeStatus
{
    [Description("启用")]
    [EnumString("enabled")]
    [JsonProperty("enabled")]
    Enabled,
    [Description("禁用")]
    [EnumString("disabled")]
    [JsonProperty("disabled")]
    Disabled
}