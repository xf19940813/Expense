using dy.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dy.Common.Config
{
    public class WxPayConfig
    {
        /// <summary>
        /// 商户账号appid
        /// </summary>
        public static string AppID = "wx2b827ba2770c4e19";

        /// <summary>
        /// 商户号
        /// </summary>
        public static string MchId = "1229789302";

        /// <summary>
        ///  随机字符串不长于 32 位
        /// </summary>
        public static string nonceStr = WxPayCore.CreateRandomNum(32).ToUpper();

        /// <summary>
        /// 商户订单号
        /// </summary>
        public static string applyNo = WxPayCore.GenerateOutTradeNo();

        /// <summary>
        /// 是否强制校验用户真是姓名，NO_CHECK：不校验真实姓名
        /// </summary>
        public static string NOCheckRealName = "NO_CHECK";  //FORCE_CHECK：强校验真实姓名

        /// <summary>
        /// 企业付款备注
        /// </summary>
        public static string PayDescription = "报销";

        /// <summary>
        /// 企业付款IP地址，当前商家接口服务所在IP地址
        /// </summary>
        public static string EnterpriseIPAddress = WxPayCore.GetPublicIP();

        /// <summary>
        /// 签名信息 WxPayModel.WxMerchantKey为获取微信商户平台密钥
        /// </summary>
        public static string WxMerchantKey = "w36AGhornLJ1zwF6QfUzSdgq1d375WqI";

        /// <summary>
        /// /微信API证书相对路径
        /// </summary>
        public static string SSLCERT_PATH = Directory.GetCurrentDirectory() + "/cert/apiclient_cert.p12";

        public static string EnterpriseWxPay = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";

    }
}
