using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class DingTalMessage
    {
        public string MessageUrl { get; set; }
        public string PicUrl { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
    public class DingTalkAt
    {
        private bool isAtAll = false;
        private List<string> atMobiles = new List<string>();
        private List<string> atUserIds=new List<string>();

        public List<string> AtMobiles { get => atMobiles; set => atMobiles = value; }
        public List<string> AtUserIds { get => atUserIds; set => atUserIds = value; }
        public bool IsAtAll { get => isAtAll; set => isAtAll = value; }
    }
    public class DingTalkRequest
    {
        private DingTalkAt at = new DingTalkAt();
        private DingTalMessage ding=new DingTalMessage();

        public string WebHook { get; set; }
        public string Secret { get; set; }
        public DingTalMessage Ding { get => ding; set => ding = value; }
        public DingTalkAt At { get => at; set => at = value; }
    }
    public class DingContentMessage
    {
        public string Content { get; set; }
    }
    public  class DingContentRequest
    {
        private DingTalkAt at = new DingTalkAt();
        private DingContentMessage ding = new DingContentMessage();

        public string WebHook { get; set; }
        public string Secret { get; set; }
        public DingContentMessage Ding { get => ding; set => ding = value; }
        public DingTalkAt At { get => at; set => at = value; }
    }
}
