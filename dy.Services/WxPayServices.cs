using dy.Common.Helper;
using dy.IRepository;
using dy.IServices;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Services
{
    public class WxPayServices : BaseServices<PaymentRecord>, IWxPayServices
    {
        private readonly IWxPayRepository _wxpayDal;

        public WxPayServices(IBaseRepository<PaymentRecord> baseRepository, IWxPayRepository wxpayDal)
            : base(baseRepository)
        {
            _wxpayDal = wxpayDal;
        }

        /// <summary>
        /// 付款到零钱-线上
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OperatorOpenId">操作人的唯一标识</param>
        /// <returns></returns>
        public WxPayResult PostPaymentToChange(WxPayDto dto, string OperatorOpenId)
        {
            return _wxpayDal.PostPaymentToChange(dto, OperatorOpenId);
        }

        /// <summary>
        /// 线下付款
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OperatorOpenId"></param>
        /// <returns></returns>
        public async Task<bool> PostOfflinePaymentAsync(OfflinePaymentDto dto, string OperatorOpenId)
        {
            return await _wxpayDal.PostOfflinePaymentAsync(dto, OperatorOpenId);
        }
    }
}
