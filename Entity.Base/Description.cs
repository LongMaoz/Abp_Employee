using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class DescriptionAttribute : Attribute
    {
        private string _description;

        public DescriptionAttribute(string desc)
        {
            _description = desc;
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }
    }
    public class EnumDicEntity
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Desc { get; set; }
    }
}
