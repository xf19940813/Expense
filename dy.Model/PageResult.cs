using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model
{
    public class PageResult<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public List<T> data { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 一也多少条
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 总共多少条
        /// </summary>
        public int totalCount { get; set; }

    }
}
