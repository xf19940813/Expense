using dy.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Helper
{
    public class FileStatusCode
    {
        /// <summary>
        /// 返回结果代码
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 返回结果消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回图片路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 返回文件夹路径
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// 返回图片路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// 返回文件名
        /// </summary>
        public string FileName { get; set; }

    }
}
