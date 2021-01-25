using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Helper
{

    public class TokenModel
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }
    }
    
    public class TokenInfo : TokenModel
    {
        /// <summary>
        /// 会话密钥
        /// </summary>
        public string SessionKey { get; set; }
    }

}
