using dy.Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace dy.Common.Helper
{
    public class WxPayCore
    {
        public static string SIGN_TYPE_MD5 = "MD5";
        public static string SIGN_TYPE_HMAC_SHA256 = "HMAC-SHA256";

        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private static SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /// <summary>
        /// 获取XML值
        /// </summary>
        /// <param name="strXml">XML字符串</param>
        /// <param name="strData">节点值</param>
        /// <returns></returns>
        public static string GetXmlValue(string strXml, string strData)
        {
            string xmlValue = string.Empty;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(strXml);
            var selectSingleNode = xmlDocument.DocumentElement.SelectSingleNode(strData);
            if (selectSingleNode != null)
            {
                xmlValue = selectSingleNode.InnerText;
            }
            return xmlValue;
        }

        /// <summary>
        /// post提交至微信服务器（需要微信API证书）
        /// </summary>
        /// <param name="url">微信企业支付路径</param>
        /// <param name="xmlParam">服务提交的签名参数</param>
        /// <returns></returns>
        public static string HttpPostResponseWxPay(string url, string xmlParam)
        {
            //微信API证书相对路径
            string SSLCERT_PATH = "";
            string SSLCERT_PASSWORD = "";

            //微信API证书相对路径
            SSLCERT_PATH = WxPayConfig.SSLCERT_PATH;
            SSLCERT_PASSWORD = WxPayConfig.MchId;

            //垃圾回收，回收没有正常关闭的http连接
            System.GC.Collect();

            //微信服务器返回结果
            string result = "";

            //HTTP请求对象
            HttpWebRequest request = null;

            //HTTP响应对象
            HttpWebResponse response = null;

            //数据流对象
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;

                //设置https验证方式，是否为https请求
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 60000;  //设置超时时间 默认时长为100秒（100,000 ms）              
                request.ContentType = "text/xml"; //设置POST的数据类型和长度
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlParam);
                request.ContentLength = data.Length;


                //string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

                X509Certificate2 cert = new X509Certificate2(SSLCERT_PATH, SSLCERT_PASSWORD, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                //添加证书凭据
                request.ClientCertificates.Add(cert);

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();

            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        /// <summary>
        /// 生产随机数
        /// </summary>
        /// <param name="NumCount"></param>
        /// <returns></returns>
        public static string CreateRandomNum(int NumCount)
        {
            string allChar = "2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,j,k,m,n,o,p,q,s,t,u,w,x,y,z";
            string[] allCharArray = allChar.Split(',');//拆分成数组
            string randomNum = "";
            int temp = -1;                             //记录上次随机数的数值，尽量避免产生几个相同的随机数
            Random rand = new Random();
            for (int i = 0; i < NumCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomNum(NumCount);
                }
                temp = t;
                randomNum += allCharArray[t];
            }
            return randomNum;
        }

        /// <summary>
        /// 根据当前系统时间加随机序列来生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", WxPayConfig.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }


        /// <summary>
        /// 集合转换XML数据 (拼接成XML请求数据)
        /// </summary>
        /// <param name="strParam">参数</param>
        /// <returns></returns>
        public static string CreateXmlParam(Dictionary<string, string> strParam)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("<xml>");
                foreach (KeyValuePair<string, string> k in strParam)
                {
                    if (k.Key == "attach" || k.Key == "body" || k.Key == "sign")
                    {
                        sb.Append("<" + k.Key + "><![CDATA[" + k.Value + "]]></" + k.Key + ">");
                    }
                    else
                    {
                        sb.Append("<" + k.Key + ">" + k.Value + "</" + k.Key + ">");
                    }
                }
                sb.Append("</xml>");
            }
            catch (Exception ex)
            {
                throw ex;
                // AddLog("PayHelper", "CreateXmlParam", ex.Message, ex);
            }
            var a = sb.ToString();
            return sb.ToString();
        }

        /// <summary>
        /// 成签名，详见签名生成算法
        /// </summary>
        /// <param name="signType"></param>
        /// <returns></returns>
        public static string MakeSign(string signType)
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + WxPayConfig.WxMerchantKey;
            if (signType == SIGN_TYPE_MD5)
            {
                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                //所有字符转为大写
                return sb.ToString().ToUpper();
            }
            else if (signType == SIGN_TYPE_HMAC_SHA256)
            {
                return CalcHMACSHA256Hash(str, WxPayConfig.WxMerchantKey);
            }
            else
            {
                throw new WxPayException("sign_type 不合法");
            }
        }



        public static string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        private static string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }

        /// <summary>
        /// 微信支付MD5签名算法，ASCII码字典序排序0,A,B,a,b
        /// </summary>
        /// <param name="InDict">待签名名键值对</param>
        /// <param name="TenPayV3_Key">用于签名的Key</param>
        /// <returns>MD5签名字符串</returns>
        public static string WePaySign(IDictionary<string, string> InDict, string TenPayV3_Key)
        {
            string[] arrKeys = InDict.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);  //参数名ASCII码从小到大排序；0,A,B,a,b;

            var StrA = new StringBuilder();

            foreach (var key in arrKeys)
            {
                string value = InDict[key];
                if (!String.IsNullOrEmpty(value)) //空值不参与签名
                {
                    StrA.Append(key + "=")
                       .Append(value + "&");
                }
            }

            //foreach (var item in InDict.OrderBy(x => x.Key))//参数名字典序；0,A,a,B,b;
            //{
            //    if (!String.IsNullOrEmpty(item.Value)) //空值不参与签名
            //    {
            //        StrA.Append(item.Key + "=")
            //           .Append(item.Value + "&");
            //    }
            //}

            StrA.Append("key=" + TenPayV3_Key); //注：key为商户平台设置的密钥key
            return GetMD5Hash(StrA.ToString()).ToUpper();
        }

        public static string GetMD5Hash(String str)
        {
            //把字符串转换成字节数组
            byte[] buffer = Encoding.Default.GetBytes(str);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //md5加密
            byte[] cryptBuffer = md5.ComputeHash(buffer);
            string s = "";
            //把每一个字节 0-255，转换成两位16进制数     
            for (int i = 0; i < cryptBuffer.Length; i++)
            {
                //大X转黄的是大写字母，小X转换的是小写字母
                s += cryptBuffer[i].ToString("x2");
            }
            return s;
        }


        public static string GetPublicIP()
        {

            //外网(公网)IP
            Stream stream = null;
            StreamReader streamReader = null;
            IPAddress PublicIP;
            try
            {
                stream = WebRequest.Create("https://www.ipip5.com/").GetResponse().GetResponseStream();
                streamReader = new StreamReader(stream, Encoding.UTF8);
                var str = streamReader.ReadToEnd();
                int first = str.IndexOf("<span class=\"c-ip\">") + 19;
                int last = str.IndexOf("</span>", first);
                var ip = str.Substring(first, last - first);
                PublicIP = IPAddress.Parse(ip);       //这里就得到了
            }
            catch (Exception ex)
            {
                throw new WxPayException($"出错了，{ex.Message}。获取失败");
            }
            finally
            {
                streamReader?.Dispose();
                stream?.Dispose();
            }

            return PublicIP.ToString();
        }

        /// <summary>
        /// 获取根路径
        /// </summary>
        /// <returns></returns>
        public static string GetDirectory()
        {
            string path = Directory.GetCurrentDirectory();
            string newPath = string.Empty;
            if(!String.IsNullOrEmpty(path))
            {
                newPath = path.Replace(@"\\", @"/");
            }
            return newPath;
        }
    }
}
