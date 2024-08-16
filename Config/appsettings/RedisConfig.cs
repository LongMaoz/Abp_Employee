using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Config
{
    public class RedisConfig
    {
        private string _redisSysCustomKey= "ERP";
        private string _redisHost="127.0.0.1";
        private int _redisPort=6379;
        private string _userName;
        private string _passWord;
        private bool _abortConnect= false;
        private int _connectRetry=3;
        private int _connectTimeout= 3000;
        private int _syncTimeout= 3000;
        private int _responseTimeout= 3000;
        private int _redisDataBaseIndex=5;

        public string RedisSysCustomKey { get => _redisSysCustomKey; set => _redisSysCustomKey = value; }
        public string RedisHost { get => _redisHost; set => _redisHost = value; }
        public int RedisPort { get => _redisPort; set => _redisPort = value; }
        public string UserName { get => _userName; set => _userName = value; }
        public string PassWord { get => _passWord; set => _passWord = value; }
        public bool AbortConnect { get => _abortConnect; set => _abortConnect = value; }
        public int ConnectRetry { get => _connectRetry; set => _connectRetry = value; }
        public int ConnectTimeout { get => _connectTimeout; set => _connectTimeout = value; }
        public int SyncTimeout { get => _syncTimeout; set => _syncTimeout = value; }
        public int ResponseTimeout { get => _responseTimeout; set => _responseTimeout = value; }

        public int RedisDataBaseIndex { get => _redisDataBaseIndex; set => _redisDataBaseIndex = value; }

    }
}
