using System;
using System.Collections.Generic;
using System.Text;

namespace Config
{
    public class ProgramConfig
    {
        private bool _isConsulService = false;
        private bool _isConsulKV = false;
        private string _serverUrls= "http://*:2080";
        private string _serviceTag;
        private int _intervalCheckKV = 5;   //检验kv是否有参数的间隔 以及 当参数改变时检验的间隔
        private bool _isCores = false; //是否限定访问url
        private string _virtualPath = "/api"; //二级虚拟目录
        private int _servicePort=0;


        /// <summary>
        /// 程序注册IP
        /// </summary>
        public string ServiceIp { get; set; }
        /// <summary>
        /// 程序注册端口
        /// </summary>
        public int ServicePort { get => _servicePort; set => _servicePort = value; }
        public string ServiceName { get; set; }
        public string ServiceID { get; set; }
        public string ServiceTag { get { return !string.IsNullOrEmpty(_serviceTag) ? _serviceTag : this.ServiceName; } set => _serviceTag = value; }

        public string ServerUrls { get => _serverUrls; set => _serverUrls = value; }

        public bool IsConsulService { get => _isConsulService; set => _isConsulService = value; }
        public bool IsConsulKV { get => _isConsulKV; set => _isConsulKV = value; }
        public int IntervalCheckKV { get => _intervalCheckKV; set => _intervalCheckKV = value; }
        public string Cores { get; set; }
        public bool IsCores { get => _isCores; set => _isCores = value; }
        public string VirtualPath { get => _virtualPath; set => _virtualPath = value; }


    }
}
