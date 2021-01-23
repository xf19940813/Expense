using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dy.Api.Controllers;
using dy.Api.Log;
using dy.Common.Config;
using dy.Common.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.WeChat
{
    /// <summary>
    /// 图像处理
    /// </summary>
    public class QRCodeController : BaseController
    {
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 图片保存路径
        /// </summary>
        private readonly string basepath = Directory.GetCurrentDirectory() + "/images/";

        /// <summary>
        /// 图像处理
        /// </summary>
        /// <param name="loggerHelper"></param>
        public QRCodeController(ILoggerHelper loggerHelper)
        {
            _logger = loggerHelper;
        }

        /// <summary>
        /// 获取发票信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetQRCodeInfo")]
        public IActionResult GetQRCodeInfo(string fileName)
        {
            try
            {
                var data = WxHelper.GetQRCodeInfo(fileName);
                return SuccessData(data);
            }
            catch(Exception err)
            {
                _logger.Error(typeof(QRCodeController), "获取发票信息失败!", new Exception(err.Message));
                return FailedMsg("获取发票信息失败！" + err.Message);
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UploadFile")]
        public FileStatusCode UploadFile([FromForm]IFormCollection collection)
        {
            FileStatusCode result = new FileStatusCode();
            var imgpath = collection["imgpath"].ToString();
            var files = collection.Files;
            var fname = "";
            var path = "";
            string newFileName = string.Empty;
            try
            {
                foreach (var file in files)
                {
                    if(file.Length <=0 )
                    {
                        continue;
                    }

                    if (!Directory.Exists(basepath))
                    {
                        Directory.CreateDirectory(basepath);
                    }

                    fname = file.FileName;
                    string type = fname.Substring(fname.LastIndexOf(".") + 1);
                    if (fname != "")
                    {
                        newFileName = Guid.NewGuid().ToString().Replace("-", "") + "." + type;
                    }


                    path = Path.Combine(basepath, imgpath + newFileName);

                    
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fs);
                        //retvalue++;
                    }
                }

                result.Status = "200";
                result.Message = $"图片 {fname} 上传成功";
                result.ImgUrl = ImgConfig.img_url + newFileName;
            }
            catch (Exception ex)
            {
                result.Status = "400";
                result.Message = ex.Message;
            }

            return result;
        }

    }
}
