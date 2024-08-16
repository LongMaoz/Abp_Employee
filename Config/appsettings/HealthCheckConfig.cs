using System;
using System.Collections.Generic;
using System.Text;

namespace Config
{
    public class HealthCheckConfig
    {
        private int messageConsumerError = 20;
        private int rabbitmqError = 20;

        public int MessageConsumerError { get => messageConsumerError; set => messageConsumerError = value; }
        public int RabbitmqError { get => rabbitmqError; set => rabbitmqError = value; }
    }
}
