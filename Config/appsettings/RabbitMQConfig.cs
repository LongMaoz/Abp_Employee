using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Config
{


    public class RabbitMQConfig
    {
        private string _hostName = "127.0.0.1";
        private string _userName = "guest";
        private string _password = "guest";
        private int _port = 5672;
        private string _virtualHost = "/";
        private int _retry = 2;
        private string _exchangeName = "erp.exchange";
        private int _expireTime = 30000; //30秒
        private int _tTLTime = 20000;
        private int _maxCount = 50;
        private string _queueName;
        private int _prefetchSize = 0;
        private int _prefetchCount = 50;
        private bool _chanelGlobal = false;
        private List<int> _tTLTimeSecondArray = new List<int> { 20, 1800, 3600, 10800, 10800, 14400, 14400, 18000, 18000, 21600, 25200, 28800, 32400, 36000, 36000 };//20秒，半小时，1.5，4.5，7.5，11.5，15.5，20.5，25.5，31.5
        private int _retrySpace = 180;
        private int _updateSpaceHour = 10;
        private int _consumerRetrySpace = 5;

        public string HostName { get => _hostName; set => _hostName = value; }
        public string UserName { get => _userName; set => _userName = value; }
        public string Password { get => _password; set => _password = value; }
        public int Port { get => _port; set => _port = value; }
        public string VirtualHost { get => _virtualHost; set => _virtualHost = value; }
        public string ExchangeName { get => _exchangeName; set => _exchangeName = value; }
        public int Retry { get => _retry; set => _retry = value; }
        public int ExpireTime { get => _expireTime; set => _expireTime = value; }
        public int TTLTime { get => _tTLTime; set => _tTLTime = value; }
        /// <summary>
        /// 消息的最大转发次数
        /// </summary>
        public int MaxCount { get => _maxCount; set => _maxCount = value; }
        public string QueueName { get => _queueName; set => _queueName = value; }

        public int PrefetchSize { get => _prefetchSize; set => _prefetchSize = value; }
        public int PrefetchCount { get => _prefetchCount; set => _prefetchCount = value; }
        public bool ChanelGlobal { get => _chanelGlobal; set => _chanelGlobal = value; }
        public List<int> TTLTimeSecondArray { get => _tTLTimeSecondArray; set => _tTLTimeSecondArray = value; }
        /// <summary>
        /// 消息队列持久链接断开时重连间隔 单位秒
        /// </summary>
        public int RetrySpace { get => _retrySpace; set => _retrySpace = value; }

        public int ConsumerRetrySpace { get => _consumerRetrySpace; set => _consumerRetrySpace = value; }

        public int UpdateSpaceHour { get => _updateSpaceHour; set => _updateSpaceHour = value; }
        public List<string> Urls { get; set; }

    }
    //public class RabbitMQExtraConfig
    //{
    //    public List<RabbitMQBaseConfig> RabbitMQExtras { get; set; }
    //}
    //public class RabbitMQBaseConfig
    //{
    //    private string _hostName = "127.0.0.1";
    //    private string _userName = "guest";
    //    private string _password = "guest";
    //    private int _port = 5672;
    //    private string _virtualHost = "/";
    //    public string HostName { get => _hostName; set => _hostName = value; }
    //    public string UserName { get => _userName; set => _userName = value; }
    //    public string Password { get => _password; set => _password = value; }
    //    public int Port { get => _port; set => _port = value; }
    //    public string VirtualHost { get => _virtualHost; set => _virtualHost = value; }
    //}

}
