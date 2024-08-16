using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class WeChatMsgContent
    {
        [JsonProperty("touser")]
        public string ToUser { get; set; }

        [JsonProperty("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 可不填写 由参数配置
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        //[JsonProperty("topcolor")]
        //public string TopColor { get; set; } = "#000000";

        [JsonProperty("data")]
        public Dictionary<string, WeChatString> Data { get; set; }
    }
    public class WeChatString
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; } = "#000000";
    }
}
