using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    /// <summary>
    /// 发送单条短信
    /// </summary>
    public class SmsChannelContent
    {
        /// <summary>
        /// 被叫号码 必填
        /// </summary>
        public string PhoneNumbers { get; set; }
        /// <summary>
        /// 模板 channel:通道(1--阿里云 2--腾讯)
        /// </summary>
        public List<TemplateModel> TemplateCode { get; set; }
        /// <summary>
        /// 模板参数
        /// </summary>
        public List<ParamsModel> TemplateParam { get; set; }
    }
    public class TemplateModel
    {
        /// <summary>
        /// 通道
        /// </summary>
        public ChannelType Channel { get; set; }
        /// <summary>
        /// 模板编号
        /// </summary>
        public string TemplateCode { get; set; }
    }
    public class ParamsModel
    {
        /// <summary>
        /// 通道
        /// </summary>
        public ChannelType Channel { get; set; }
        public Dictionary<string, string> TemplateParam { get; set; }
    }
    public enum ChannelType
    {
        Aliyun = 1,
        Tencent = 2,
        Qiniu = 3
    }
    public class SmsChannelBatchContent
    {
        public List<string> PhoneNumberJson { get; set; }
        /// <summary>
        /// 模板参数
        /// </summary>
        public List<ParamsBatchModel> TemplateParamJson { get; set; }
        public string Remark { get; set; }

    }
    public class ParamsBatchModel
    {
        /// <summary>
        /// 通道
        /// </summary>
        public ChannelType Channel { get; set; }
        public string TemplateCode { get; set; }
        public List<Dictionary<string, string>> TemplateParamJson { get; set; }
        /// <summary>
        /// 上行短信扩展码,无特殊需要此字段的用户请忽略此字段
        /// </summary>
        public List<string> SmsUpExtendCodeJson { get; set; }
        /// <summary>
        /// 短信签名 可不填 默认为兰拓相机租赁
        /// </summary>
        public List<string> SignNameJson { get; set; }
    }
}
