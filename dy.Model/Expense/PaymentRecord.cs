using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Expense
{
    /// <summary>
    /// 付款记录
    /// </summary>
    [SugarTable("tab_PaymentRecord")]
    public class PaymentRecord
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 报销单Id
        /// </summary>
        public string ExpenseId { get; set; }
        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// 收款用户Id
        /// </summary>
        public string PayeeUserId { get; set; }

        /// <summary>
        /// 操作用户Id
        /// </summary>
        public string OperatorUserId { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public short? PaymentType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string Mchid { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string NonceStr { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string PartnerTradeNo { get; set; }

        /// <summary>
        /// 付款单号
        /// </summary>
        public string PaymentNo { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
