using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class UnitResult<T>
    {
        private bool _result=true;

        public bool result { get => _result; set => _result = value; }
        public string message { get; set; }
        public T obj { get; set; }
        public UnitResult()
        {

        }
        public UnitResult(bool _result, string _message, T _obj)
        {
            result = _result;
            message = _message;
            obj = _obj;
        }
    }
    public class UnitResult<T,K>
    {
        private bool _result = true;

        public bool result { get => _result; set => _result = value; }
        public string message { get; set; }
        public T obj { get; set; }
        public K extra { get; set; }
        public UnitResult()
        {

        }
        public UnitResult(bool _result, string _message, T _obj,K _extra)
        {
            result = _result;
            message = _message;
            obj = _obj;
            extra = _extra;
        }
    }
    public class UnitResult<T, K,M>
    {
        private bool _result = true;

        public bool result { get => _result; set => _result = value; }
        public string message { get; set; }
        public T obj { get; set; }
        public K extra { get; set; }
        public M external { get; set; }
        public UnitResult()
        {

        }
        public UnitResult(bool _result, string _message, T _obj, K _extra,M _external)
        {
            result = _result;
            message = _message;
            obj = _obj;
            extra = _extra;
            external = _external;
        }
    }
    public class UnitResult
    {
        private bool _result=true;

        public bool result { get => _result; set => _result = value; }
        public string message { get; set; }
        public UnitResult()
        {

        }
        public UnitResult(bool _result, string _message = "")
        {
            result = _result;
            message = _message;
        }
    }
}
