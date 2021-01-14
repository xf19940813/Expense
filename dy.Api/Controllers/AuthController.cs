using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dy.Api.Log;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.Controllers
{
    /// <summary>
    /// 获取Token
    /// </summary>
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class AuthController : BaseController
    {
        private readonly ILoggerHelper _logger;
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisCacheManager"></param>
        /// <param name="loggerHelper"></param>
        public AuthController(IRedisCacheManager redisCacheManager, ILoggerHelper loggerHelper)
        {
            _redisCacheManager = redisCacheManager;
            _logger = loggerHelper;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        [HttpGet("GenerateJWTToken")]
        public IActionResult GenerateJWTToken()
        {
            try
            {
                bool ismyOpenId = _redisCacheManager.Get("myOpenId");
                bool ismySessionKey = _redisCacheManager.Get("mySessionKey");
                string OpenId = string.Empty;
                string SessionKey = string.Empty;
                if (ismyOpenId && ismySessionKey)
                {
                    OpenId = _redisCacheManager.GetValue("myOpenId").ToString().Trim('"');
                    SessionKey = _redisCacheManager.GetValue("mySessionKey").ToString().Trim('"');
                }

                if (OpenId == "") throw new Exception("OpenId为空，获取token失败！");

                string userId = OpenId + ";" + SessionKey;
                string jwtStr = string.Empty;
                bool suc = false;

                TokenModel tokenModel = new TokenModel { OpenId = OpenId };
                jwtStr = JwtHelper.IssueJwt(tokenModel);//登录，获取到一定规则的 Token 令牌
                suc = true;

                _redisCacheManager.Set(jwtStr, userId, TimeSpan.FromHours(2));

                return Ok(new
                {
                    success = suc,
                    token = jwtStr
                });
            }
            catch(Exception err)
            {
                _logger.Error(typeof(AuthController), "获取token失败!", new Exception(err.Message));
                return FailedMsg(err.Message);
            }
           
        }
    }
}
