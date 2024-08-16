using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Dto;

public class SearchRequestInput:LimitRequestInput
{
    [JsonProperty("keyword")]
    [SwaggerParameter("搜索关键字")]
    public string? Keyword { get; set; }
}