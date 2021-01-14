using dy.Model.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Expense
{
    /// <summary>
    /// 报销信息
    /// </summary>
    [SugarTable("tab_ExpenseInfo")]
    public class ExpenseInfo
    {
        /// <summary>
        /// 主键ID，如果是主键，此处必须指定，否则会引发InSingle(id)方法异常。
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 消费类型
        /// </summary>
        public short? ExpenseType { get; set; }

        /// <summary>
        /// 消费时间
        /// </summary>
        public DateTime ConsumeTime { get; set; }

        /// <summary>
        /// 消费项目
        /// </summary>
        public string ConsumeProject { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public short? AuditStatus { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUserId { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
