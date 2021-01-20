using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dy.Api.Controllers;
using dy.Api.Log;
using dy.Common.Config;
using dy.Common.Enum;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IServices;
using dy.Model;
using dy.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.WeChat
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public class WxPayController : BaseController
    {
        private readonly IWxPayServices _payServices;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 微信支付
        /// </summary>
        /// <param name="payServices"></param>
        /// <param name="loggerHelper"></param>
        public WxPayController(IWxPayServices payServices, ILoggerHelper loggerHelper)
        {
            _payServices = payServices;
            _logger = loggerHelper;
        }


        /// <summary>
        /// 付款到零钱-线上
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("PostPaymentToChange")]
        public IActionResult PostPaymentToChange([FromBody]WxPayDto dto)
        {
            
            string OperatorOpenId = GetOpenId();
            try
            {
                var result = _payServices.PostPaymentToChange(dto, OperatorOpenId);
                if (result.ResultCode == ResultCode.Fail) 
                {
                    _logger.Error(typeof(WxPayController), result.ResultMsg);
                }

                return Ok(result);
            }
            catch(Exception err)
            {
                _logger.Error(typeof(WxPayController), "付款到零钱失败！", new Exception(err.Message));
                return FailedMsg("付款到零钱失败! " + err.Message);
            }
        }

        /// <summary>
        /// 线下付款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("PostOfflinePaymentAsync")]
        public async Task<IActionResult> PostOfflinePaymentAsync([FromBody]OfflinePaymentDto dto)
        {
            string OperatorOpenId = GetOpenId();
            try
            {
                var data = await _payServices.PostOfflinePaymentAsync(dto, OperatorOpenId);
                return SuccessMsg("线下付款成功");
            }
            catch (Exception err)
            {
                _logger.Error(typeof(WxPayController), "线下付款失败！", new Exception(err.Message));
                return FailedMsg("线下付款失败！ " + err.Message);
            }
        }
    }
}
