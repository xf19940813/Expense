using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 添加消费信息
    /// </summary>
    public class AddExpenseInfoDto
    {
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
        public int ExpenseType { get; set; }

        /// <summary>
        /// 消费时间
        /// </summary>
        public DateTime ConsumeTime { get; set; }

        /// <summary>
        /// 消费项目
        /// </summary>
        public string ConsumeProject { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 图片名称, 示例a.jpg,b.jpg,e.jpg
        /// </summary>
        public string ImgNames { get; set; }
    }

    /// <summary>
    /// 报销单详情
    /// </summary>
    public class ExpenseInfoDetailDto
    {
        /// <summary>
        /// 审核状态
        /// </summary>
        public short? AuditStatus { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string Proposer { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 消费项目
        /// </summary>
        public string ConsumeProject { get; set; }

        /// <summary>
        /// 消费类型
        /// </summary>
        public short? ExpenseType { get; set; }

        /// <summary>
        /// 消费时间
        /// </summary>
        public DateTime ConsumeTime { get; set; }

        /// <summary>
        /// 报销时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AuditDto
    {
        /// <summary>
        /// 报销单Id
        /// </summary>
        public string Id { get; set; }
    }
}
