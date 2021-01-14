using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Expense
{
    /// <summary>
    /// 团队实体
    /// </summary>
    [SugarTable("tab_Team")]
    public class Team
    {
        /// <summary>
        ///  主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 团队信息
        /// </summary>
        public string TeamInfo { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastModifyUserId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
