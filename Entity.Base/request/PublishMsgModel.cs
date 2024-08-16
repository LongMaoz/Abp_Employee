//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Entity.Base
//{
//    public class PublishMsgModel
//    {
//        public MsgSubject MsgInfor { get; set; }
//        public string RoutingKey { get; set; }
//        public string ExchangeName { get; set; }
//        /// <summary>
//        /// 队列名称
//        /// </summary>
//        public string QueueName { get; set; }
//    }
//    public partial class MsgObject
//    {
//        public dynamic MsgContent { get; set; }
//    }
//    public class MsgSubject:MsgObject
//    {
//        private bool _isString=false;  //MsgContent 是否字符串 默认false 是object
//        public bool IsString { get => _isString; set => _isString = value; }
//        public string EventName { get; set; }
//        public Dictionary<string, object> Params { get; set; }
//    }
   
//    public class PublishDelayMsgModel
//    {
//        public string  MsgInfor { get; set; }
//        public string RoutingKey { get; set; }
//        public string ExchangeName { get; set; }
//        public string QueueName { get; set; }
//        /// <summary>
//        /// 延迟时间 毫秒 30000 3秒
//        /// </summary>
//        public int? TTLTime { get; set; }
//    }
//}
