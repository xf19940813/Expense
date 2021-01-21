using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class WxConfig
    {
        /// <summary>
        /// 小程序的appId
        /// 登录小程序可以直接看到
        /// </summary>
        public static string AppId { get; set; } = "wx2b827ba2770c4e19";
        /// <summary>
        /// 小程序的AppSecret
        /// 只能修改,不能查看
        /// </summary>
        public static string AppSecret { get; set; } = "de2e0b9e04606192df308b29e49a6a70";
        /// <summary>
        /// 微信的token 用过的都知道是干嘛的
        /// </summary>
        //public static string Access_token { get; set; } = "client_credential";

        public static string grant_type { get; set; } = "client_credential";

    }
}
