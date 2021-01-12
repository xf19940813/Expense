using dy.Common.Config;
using dy.Common.Redis;
using dy.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace dy.Common.Helper
{
    public static class WxHelper
    {
        //public WxHelper()
        public static readonly IRedisCacheManager _redisCacheManager = new RedisCacheManager();

        private const string host = "https://nvoiceocr.market.alicloudapi.com";
        private const string path = "/taxinvoice";
        private const string method = "POST";
        private const string appcode = "0c51ce2cac9d4fdb9c16b4d378e586b8";//开通服务后 买家中心-查看AppCode

        /// <summary>
        /// 获取发票信息
        /// </summary>
        /// <returns></returns>
        public static string GetQRCodeInfo(string fileName)
        {
            var querys = "";
            string base64String = WxHelper.ImageToBase64(fileName);
            var bodys = "image=" + base64String;
            var url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }
            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            //根据API的要求，定义相对应的Content-Type
            httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            string returnText = reader.ReadToEnd();
            st.Close();
            reader.Close();
            return returnText;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// Image 转成 base64
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public static string ImageToBase64(string fileName)
        {
            try
            {
                string img_path = "http://192.168.3.21:8000/images/" + fileName;
                var arr1 = img_path.Split('.');
                string extention = arr1[arr1.Length - 1];

                string base64String = "";
                System.Drawing.Imaging.ImageFormat imf = extention == "jpg" ?
                    System.Drawing.Imaging.ImageFormat.Jpeg : System.Drawing.Imaging.ImageFormat.Png;

                WebClient webClient = new WebClient();
                byte[] Bytes = webClient.DownloadData(img_path);
                using (MemoryStream ms = new MemoryStream(Bytes))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                    System.Drawing.Bitmap bmp = new Bitmap(img);
                    MemoryStream ms1 = new MemoryStream();
                    bmp.Save(ms1, imf);
                    byte[] arr = new byte[ms1.Length];
                    ms1.Position = 0;
                    ms1.Read(arr, 0, (int)ms1.Length);
                    ms1.Close();
                    base64String = "data:image/" + extention + ";base64," + Convert.ToBase64String(arr);
                }
                return base64String;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取微信用户openid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetOpenId(string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + WxConfig.AppId + "&secret=" + WxConfig.AppSecret + "&js_code=" + code + "&grant_type=authorization_code";
            WebClient wc = new System.Net.WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;
            string returnText = wc.DownloadString(url);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误  
            }

            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(returnText);
            string openid = obj["openid"].ToString();
            string sessionkey = obj["session_key"].ToString();

            _redisCacheManager.Remove("myOpenId");
            _redisCacheManager.Remove("mySessionKey");
            _redisCacheManager.Set("myOpenId", openid, TimeSpan.FromHours(2));
            _redisCacheManager.Set("mySessionKey", sessionkey, TimeSpan.FromHours(2));
            return openid;

        }

        /// <summary>
        /// 获取session_key
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetSessionKey(string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + WxConfig.AppId + "&secret=" + WxConfig.AppSecret + "&js_code=" + code + "&grant_type=authorization_code";
            WebClient wc = new System.Net.WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;
            string returnText = wc.DownloadString(url);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误  
            }
            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(returnText);
            string session_key = obj["session_key"].ToString();

            return session_key;
        }

        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=" + WxConfig.grant_type + "&appid=" + WxConfig.AppId + "&secret=" + WxConfig.AppSecret;
            WebClient wc = new System.Net.WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;
            string returnText = wc.DownloadString(url);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误  
            }
            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(returnText);
            string access_token = obj["access_token"].ToString();

            return access_token;
        }

        /// <summary>
        /// 获取用户手机号码
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <param name="session_key"></param>
        /// <returns></returns>
        public static string getPhoneNumber(string encryptedData, string iv, string session_key)
        {

            try
            {
                //判断是否是16位 如果不够补0
                //text = tests(text);
                //16进制数据转换成byte
                byte[] encryptedDatas = Convert.FromBase64String(encryptedData); // strToToHexByte(text);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(session_key); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Convert.FromBase64String(iv);// Encoding.UTF8.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedDatas, 0, encryptedDatas.Length);
                string results = Encoding.Default.GetString(plainText);

                dynamic model = Newtonsoft.Json.Linq.JToken.Parse(results) as dynamic;
                string phoneNumber = model.phoneNumber;
                //return model.phoneNumber;
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return "";
                }
                return phoneNumber;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        

        public static string GetResponse(string url)
        {
            WebClient wc = new System.Net.WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = Encoding.UTF8;
            string returnText = wc.DownloadString(url);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误  
            }
            return returnText;
        }
    }
}
