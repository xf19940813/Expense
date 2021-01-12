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
    }
}
