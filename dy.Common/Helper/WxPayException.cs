using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Helper
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg)
        {

        }
    }
}
