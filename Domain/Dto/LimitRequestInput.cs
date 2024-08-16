using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class LimitRequestInput
    {
        [JsonProperty("offset")]
        [DefaultValue(0)]
        [SwaggerParameter("位移", Required = true)]
        public int Offset { get; set; } = 0;

        [JsonProperty("limit")]
        [DefaultValue(10)]
        [SwaggerParameter("每条页数", Required = true)]
        public int Limit { get; set; } = 10;
    }
}
