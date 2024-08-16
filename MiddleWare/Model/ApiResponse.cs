using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddleWare.Model
{
    public class ApiResponse<T>
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }
        [JsonProperty("status_message")]
        public string? StatusMessage { get; set; }
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty("data")]
        public T Data { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(int status_code, T data, string status_message = "")
        {
            this.Timestamp = TimeSpan();
            this.StatusCode = status_code;
            this.Data = data;
            this.StatusMessage = status_message;
        }

        public long TimeSpan()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
