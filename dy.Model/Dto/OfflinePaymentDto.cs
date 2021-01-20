using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 线下付款
    /// </summary>
    public class OfflinePaymentDto
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
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public short? PaymentType { get; set; }  

    }
}
