using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    /// <summary>
    /// 延迟消息格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelayMsgContent<T> where T : class
    {
        /// <summary>
        /// 消息主体
        /// </summary>
        public T MsgContent { get; set; }
        /// <summary>
        ///延迟时间 秒
        /// </summary>
        public int DelayTime { get; set; }
    }
}
