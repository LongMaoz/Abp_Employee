using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class GrpcErpApi
    {
        private int _retry = 1;
        private int _retrySpace = 1;
        private string _errorRegx = "StatusCode = (?< code >.*?), Detail =\"(?<key>.*?)\"";
        private string _apiUrlWareHouse;
        private NacosConfigKey employeeKey = new() { Host = "service:employee:host", Port = "service:employee:port" };
        private NacosConfigKey userKey = new() { Host = "service:user:host", Port = "service:user:port" };
        private NacosConfigKey helpKey = new() { Host = "service:help:host", Port = "service:help:port" };
        private NacosConfigKey productKey = new() { Host = "service:product:host", Port = "service:product:port" };
        private NacosConfigKey equipmentKey = new() { Host = "service:equipment:host", Port = "service:equipment:port" };
        private NacosConfigKey logisticsKey = new() { Host = "service:logistics:host", Port = "service:logistics:port" };
        private string _logLevel = "Information";
        public string ApiUrlWareHouse { get => _apiUrlWareHouse; set => _apiUrlWareHouse = value; }
        public NacosConfigKey UserKey { get => userKey; set => userKey = value; }
        public NacosConfigKey EmployeeKey { get => employeeKey; set => employeeKey = value; }
        public NacosConfigKey EquipmentKey { get => equipmentKey; set => equipmentKey = value; }
        public NacosConfigKey HelpCenterKey { get => helpKey; set => helpKey = value; }
        public NacosConfigKey ProductKey { get => productKey; set => productKey = value; }
        public NacosConfigKey LogisticsKey { get => logisticsKey; set => logisticsKey = value; }

        public NacosConfigKey NotifyKey = new NacosConfigKey() { Host = "service:notify:host", Port = "service:notify:port" };

        public NacosConfigKey TransferKey = new() { Host = "service:transfer:host", Port = "service:transfer:port" };

        public int TimeOut { get; set; } = 30;
        public int RetryCount { get => _retry; set => _retry = value; }
        public int RetrySpace { get => _retrySpace; set => _retrySpace = value; }
        public string ErrorRegx { get => _errorRegx; set => _errorRegx = value; }
        public string LogLevel { get => _logLevel; set => _logLevel = value; }
    }
    public class NacosConfigKey
    {
        public string Host { get; set; }
        public string Port { get; set; }
    }
}
