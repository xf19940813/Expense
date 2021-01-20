using dy.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class WxPayResult
    {
        /// <summary>
        /// 返回结果代码
        /// </summary>
        public ResultCode ResultCode { get; set; }

        /// <summary>
        /// 返回结果信息
        /// </summary>
        public string ResultMsg { get; set; }

    }
}
