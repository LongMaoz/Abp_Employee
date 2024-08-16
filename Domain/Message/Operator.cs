using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Message
{
    public class Operator
    {
        private int _id = 0;
        [JsonProperty("id")]
        public int ID { get => _id; set => _id = value; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
