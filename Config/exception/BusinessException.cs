using System;
using System.Collections.Generic;
using System.Text;

namespace Config
{
    public class GrpcException : Exception
    {
        public GrpcException(int hResult, string message)
            : base(message)
        {
            base.HResult = hResult;

        }
    }
    public class HttpGetException : Exception
    {
        public HttpGetException(int hResult, string message)
            : base(message)
        {
            base.HResult = hResult;

        }
    }
}
