using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dy.Common.Helper;
using dy.Common.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.Controllers
{
    /// <summary>
    /// 自定义路由模版
    /// 用于解决swagger文档No operations defined in spec!问题
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        IRedisCacheManager _redisCacheManager = new RedisCacheManager();

        protected virtual T CreateService<T>()
        {
            return (T)this.HttpContext.RequestServices.GetService(typeof(T));
        }

        [NonAction]
        public ContentResult JsonContent(object obj)
        {
            string json = JsonHelper.Serialize(obj);
            return base.Content(json);
        }
        [NonAction]
        public ContentResult JsonToPage<T>(PagedData<T> data = null)
            where T : class, new()
        {
            var res = new
            {
                code = 0,
                msg = "",
                count = data.TotalCount,
                data = data.Models,
                totalRow = data.TotalField
            };
            return this.JsonContent(res);
        }
        [NonAction]
        public ContentResult SuccessData(object data = null)
        {
            Result<object> result = Result.CreateResult<object>(ResultStatus.OK, data);
            return this.JsonContent(result);
        }
        [NonAction]
        public ContentResult SuccessMsg(string msg = "操作成功")
        {
            Result result = new Result(ResultStatus.OK, msg);
            return this.JsonContent(result);
        }
        [NonAction]
        public ContentResult AddSuccessData(object data, string msg = "添加成功")
        {
            Result<object> result = Result.CreateResult<object>(ResultStatus.OK, data);
            result.Msg = msg;
            return this.JsonContent(result);
        }
        [NonAction]
        public ContentResult AddSuccessMsg(string msg = "添加成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public ContentResult AddFailedMsg(string msg = "")
        {
            string result = "添加失败！" + msg;
            return this.FailedMsg(result);
        }
        [NonAction]
        public ContentResult UpdateSuccessMsg(string msg = "更新成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public ContentResult DeleteSuccessMsg(string msg = "删除成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public ContentResult AuditSuccessMsg(string msg = "审核成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public ContentResult UnAuditSuccessMsg(string msg = "取消审核成功")
        {
            return this.SuccessMsg(msg);
        }
        [NonAction]
        public ContentResult FailedMsg(string msg = null)
        {
            Result retResult = new Result(ResultStatus.Failed, msg);
            return this.JsonContent(retResult);
        }

        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [NonAction]
        public void SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [NonAction]
        public string GetSession(string key)
        {
            var value = HttpContext.Session.GetString(key);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }

        /// <summary>
        /// 根据Token获取OpenID
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string GetOpenId()
        {
            //从Header中获取Token
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isKey = _redisCacheManager.Get(tokenHeader);
            string openId = string.Empty;
            if (isKey)
            {
                //根据Token中的信息获取到OpenId
                openId = _redisCacheManager.GetValue(tokenHeader).ToString().Split(";")[0].Trim('"');
            }

            return openId;
        }

        /// <summary>
        /// 获取token中的OpenId和Session_key
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public TokenInfo GetTokenInfo()
        {
            var tokenInfo = new TokenInfo();
            //从Header中获取Token
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isKey = _redisCacheManager.Get(tokenHeader);
            tokenInfo.OpenId = string.Empty;
            tokenInfo.SessionKey = string.Empty;
            if(isKey)
            {
                tokenInfo.OpenId = _redisCacheManager.GetValue(tokenHeader).ToString().Split(";")[0].Trim('"');
                tokenInfo.SessionKey = _redisCacheManager.GetValue(tokenHeader).ToString().Split(";")[1].Trim('"');
            }

            return tokenInfo;
        }
    }
}
