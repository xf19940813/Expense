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
        public string ImgUrl { get; set; }

    }
}
