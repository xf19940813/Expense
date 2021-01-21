using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 新增附件
    /// </summary>
    public class AddSheetDto
    {
        /// <summary>
        /// 报销单Id
        /// </summary>
        public string ExpenseId { get; set; }

        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// 图片名称, 示例a.jpg,b.jpg,e.jpg
        /// </summary>
        public string ImgNames { get; set; }
    }

    /// <summary>
    /// 查询附件
    /// </summary>
    public class QuerySheetDto
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }
    }
}
