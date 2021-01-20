using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public class WxPayDto
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
        public short PaymentType { get; set; } = AppConsts.PaymentType.Promptly;

    }
}
