using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string SessionKey { get; set; }
    }
}
