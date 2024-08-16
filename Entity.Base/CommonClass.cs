using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class FilterCategory
    {
        private bool _isClass=false;

        public string Name { get; set; }
        /// <summary>
        /// 多个用逗号隔开
        /// </summary>
        public string Values { get; set; }
        public List<FilterCategory> Category { get; set; }
        public bool IsClass { get => _isClass; set => _isClass = value; }
        public string Type { get; set; }
        
    }
}
