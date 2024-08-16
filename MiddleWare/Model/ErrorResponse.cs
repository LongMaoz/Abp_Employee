using Newtonsoft.Json;

namespace MiddleWare.Model;

public class ErrorResponse
{
    [JsonProperty("status_code")]
    public int StatusCode { get; set; }

    [JsonProperty("status_message")]
    public string? StatusMessage { get; set; }

    [JsonProperty("errors")]
    public List<string>? Errors { get; set; }
}