using dy.Common.Helper;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IServices
{
    public interface IWxPayServices : IBaseServices<PaymentRecord>
    {
        /// <summary>
        /// 付款到零钱-线上
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OperatorOpenId">操作人的唯一标识</param>
        /// <returns></returns>
        WxPayResult PostPaymentToChange(WxPayDto dto, string OperatorOpenId);

        /// <summary>
        /// 线下付款
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OperatorOpenId"></param>
        /// <returns></returns>
        Task<bool> PostOfflinePaymentAsync(OfflinePaymentDto dto, string OperatorOpenId);
    }
}
