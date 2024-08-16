using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcService
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class AppCacheAttribute : Attribute
    {
        public string _module { get; set; }
        public Type _obj { get; set; }
        public int _expire { get; set; }
        public AppCacheAttribute(string module, Type obj, int expireSecond = 0)
        {
            _module = module;
            _obj = obj;
            _expire = expireSecond;
        }
    }
}
