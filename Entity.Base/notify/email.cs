using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    /// <summary>
    /// 发送邮件内容
    /// </summary>
    public class EmailContent
    {
        /// <summary>
        /// 收件人地址 多个 email 地址可以用逗号分隔，最多100个地址
        /// </summary>
        public string ToAddress { get; set; }
        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件正文
        /// </summary>
        public string HtmlBody { get; set; }
        public string Remark { get; set; }
    }
    /// <summary>
    /// 根据模板发送邮件
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// 模板编号
        /// </summary>
        public int TemplateId { get; set; }
        /// <summary>
        /// 邮件主题 可不填 默认取模板名称
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ToAddress { get; set; }
        /// <summary>
        /// 模板参数
        /// </summary>
        public Dictionary<string, string> Params { get; set; }
        public string Remark { get; set; }
    }
}
