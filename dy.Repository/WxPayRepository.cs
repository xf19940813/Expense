using AutoMapper;
using dy.Common.Config;
using dy.Common.Enum;
using dy.Common.Helper;
using dy.IRepository;
using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using dy.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Repository
{
    public class WxPayRepository : BaseRepository<PaymentRecord>, IWxPayRepository
    {
        private readonly IMapper iMapper;

        public WxPayRepository(IMapper IMapper)
        {
            iMapper = IMapper;
        }

        /// <summary>
        /// 付款到零钱-线上
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OperatorOpenId">操作人的唯一标识</param>
        /// <returns></returns>
        public WxPayResult PostPaymentToChange(WxPayDto dto, string OperatorOpenId)
        {
            WxPayResult result = new WxPayResult();

            int amount = Convert.ToInt32(dto.Amount * 100);
            if (amount < 30) throw new Exception("报销金额不能少于0.3元！");
            if (dto.ExpenseId == null) throw new Exception("报销单Id不能为空！");
            if (dto.TeamId == null) throw new Exception("团队Id不能为空！");
            if (dto.PaymentType != AppConsts.PaymentType.Promptly && dto.PaymentType != AppConsts.PaymentType.FinanceVerify)
                throw new Exception("线上支付方式只能为1或2！");

            var AuditStatus = db.Queryable<ExpenseInfo>().Where(a => a.IsDeleted == false && a.ID == dto.ExpenseId).First()?.AuditStatus;
            if (AuditStatus == AppConsts.AuditStatus.UnAudited)
                throw new Exception("此报销单还未审核，需提醒财务审核！");

            string PayeeOpenId = string.Empty; //收款人的openId
            string PayeeUserId = string.Empty;
            if (dto.PaymentType == AppConsts.PaymentType.Promptly) //立即支付
            {
                PayeeOpenId = OperatorOpenId;
                PayeeUserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == PayeeOpenId).First()?.ID;
            }

            if (dto.PaymentType == AppConsts.PaymentType.FinanceVerify) //财务审核确认之后再支付
            {
                PayeeUserId = db.Queryable<ExpenseInfo>().Where(a => a.IsDeleted == false && a.ID == dto.ExpenseId).First()?.CreateUserId;
                PayeeOpenId = db.Queryable<Wx_UserInfo>().Where(a => a.ID == PayeeUserId).First()?.OpenId;
            }

            #region 参数准备

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("mch_appid", WxPayConfig.AppID); // 小程序APPID
            parameters.Add("mchid", WxPayConfig.MchId); // 微信商户号                                     
            parameters.Add("nonce_str", WxPayConfig.nonceStr); //随机32位字符串                                     
            parameters.Add("partner_trade_no", WxPayConfig.applyNo); //商户订单号
            parameters.Add("openid", PayeeOpenId); //收款用户唯一标识
            parameters.Add("check_name", WxPayConfig.NOCheckRealName);  //是否强制校验用户真是姓名
            parameters.Add("amount", amount.ToString()); //支付金额
            parameters.Add("desc", WxPayConfig.PayDescription); //企业付款备注
            parameters.Add("spbill_create_ip", WxPayConfig.EnterpriseIPAddress); //企业付款IP地址，当前商家接口服务所在IP地址

            string sign = WxPayCore.WePaySign(parameters, WxPayConfig.WxMerchantKey);
            parameters.Add("sign", sign);
            ;
            #endregion

            #region 向微信提交付款申请，获取返回值，并解析返回值
            // 获取接口返回xml字符串，WxPayModel.EnterpriseWxPay为企业支付接口地址，即：https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers
            string return_xml_msg = WxPayCore.HttpPostResponseWxPay(WxPayConfig.EnterpriseWxPay, WxPayCore.CreateXmlParam(parameters));


            string return_code = WxPayCore.GetXmlValue(return_xml_msg, "return_code"); // 获取提交状态
            string return_msg = WxPayCore.GetXmlValue(return_xml_msg, "return_msg"); // 获取返回信息
            string mch_appid = WxPayCore.GetXmlValue(return_xml_msg, "mch_appid");  // 获取商户appid
            string mchid = WxPayCore.GetXmlValue(return_xml_msg, "mchid"); // 获取商户号
            string nonce_str = WxPayCore.GetXmlValue(return_xml_msg, "nonce_str"); // 获取随机数
            string result_code = WxPayCore.GetXmlValue(return_xml_msg, "result_code"); // 获取业务结果
            string err_code = WxPayCore.GetXmlValue(return_xml_msg, "err_code"); // 获取业务结果错误代码
            string err_code_des = WxPayCore.GetXmlValue(return_xml_msg, "err_code_des"); // 获取业务结果错误描述
            string partner_trade_no = WxPayCore.GetXmlValue(return_xml_msg, "partner_trade_no"); // 获取业务结果商户订单号
            string payment_no = WxPayCore.GetXmlValue(return_xml_msg, "payment_no"); // 获取业务结果微信付款单号
            string payment_time = WxPayCore.GetXmlValue(return_xml_msg, "payment_time"); // 获取业务结果微信付款成功时间

            // 针对提交结果进行判断，是否成功提交至微信后台
            if (return_code == "SUCCESS")
            {
                // 微信是否返回付款成功业务处理结果
                if (result_code == "SUCCESS")
                {
                    result.ResultCode = ResultCode.OK;
                    result.ResultMsg = "付款成功，请提醒相关人员注意核对！";

                    PaymentRecord paymentRecord = new PaymentRecord();
                    paymentRecord.ID = IdHelper.CreateGuid();
                    paymentRecord.ExpenseId = dto.ExpenseId;
                    paymentRecord.TeamId = dto.TeamId;
                    paymentRecord.PayeeUserId = PayeeUserId;
                    paymentRecord.PaymentType = dto.PaymentType;
                    paymentRecord.Amount = dto.Amount;
                    paymentRecord.Mchid = mchid;
                    paymentRecord.NonceStr = nonce_str;
                    paymentRecord.PartnerTradeNo = partner_trade_no;
                    paymentRecord.PaymentNo = payment_no;
                    paymentRecord.PaymentTime = Convert.ToDateTime(payment_time);
                    paymentRecord.IsDeleted = false;

                    if (dto.PaymentType == AppConsts.PaymentType.FinanceVerify) //财务审核确认之后再支付
                    {
                        var OperatorUserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == OperatorOpenId).First()?.ID;
                        paymentRecord.OperatorUserId = OperatorUserId;
                    }

                    //添加付款记录
                    var i = db.Insertable(paymentRecord).ExecuteCommand();
                    if(i > 0)
                    {
                        db.Updateable<ExpenseInfo>(e => e.AuditStatus == AppConsts.AuditStatus.Finished).Where(e => e.ID == dto.ExpenseId).ExecuteCommand();
                    }
                }
                else
                {
                    // 加入一些日志信息
                    result.ResultCode = ResultCode.Fail;
                    result.ResultMsg = "微信支付业务处理错误，错误代码：【" + err_code + "】，错误信息：【" + err_code_des + "】";
                }
            }
            else
            {
                // 加入一些日志信息
                result.ResultCode = ResultCode.Fail;
                result.ResultMsg = return_msg;
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 线下付款
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OperatorOpenId"></param>
        /// <returns></returns>
        public async Task<bool> PostOfflinePaymentAsync(OfflinePaymentDto dto, string OperatorOpenId)
        {
            if (dto.PaymentType != AppConsts.PaymentType.Offline)
                throw new Exception("线下付款方式只能为3！");

            string PayeeUserId = string.Empty; //收款人UserId
            string OperatorUserId = string.Empty; //操作人UserId
            if (dto.PaymentType == AppConsts.PaymentType.Offline)
            {
                if (dto.ExpenseId == null) throw new Exception("报销单Id不能为空！");

                PayeeUserId = db.Queryable<ExpenseInfo>().Where(a => a.IsDeleted == false && a.ID == dto.ExpenseId).First()?.CreateUserId;
                OperatorUserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == OperatorOpenId).First()?.ID;

            }

            PaymentRecord paymentRecord = iMapper.Map<PaymentRecord>(dto);
            paymentRecord.ID = IdHelper.CreateGuid();
            paymentRecord.ExpenseId = dto.ExpenseId;
            paymentRecord.TeamId = dto.TeamId;
            paymentRecord.PayeeUserId = PayeeUserId;
            paymentRecord.OperatorUserId = OperatorUserId;
            paymentRecord.PaymentType = dto.PaymentType;
            paymentRecord.Amount = dto.Amount;
            paymentRecord.IsDeleted = false;

            return await Task.Run(() =>
            {
                var result = db.Insertable(paymentRecord).ExecuteCommand();
                if (result > 0)
                {
                    db.Updateable<ExpenseInfo>(e => e.AuditStatus == AppConsts.AuditStatus.Finished).Where(e => e.ID == dto.ExpenseId).ExecuteCommand();
                }

                return result > 0;
            });
        }
    }
}
