using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class CallTTSContent
    {
        //"aliyun:TTS_145585715 tencent template:384609";

        /// <summary>
        /// 被叫号码 必填
        /// </summary>
        public string CalledNumber { get; set; }
        /// <summary>
        /// 模板 channel:通道(1--阿里云 2--腾讯)
        /// </summary>
        public List<TemplateModel> TemplateCode { get; set; }
        /// <summary>
        /// 模板参数
        /// </summary>
        public List<ParamsModel> TemplateParam { get; set; }
    }
}
