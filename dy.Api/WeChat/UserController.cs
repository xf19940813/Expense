using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dy.Api.Controllers;
using dy.Api.Log;
using dy.Common.Config;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IServices;
using dy.Model.Dto;
using dy.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace dy.Api.WeChat
{

    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IUserServices _userService;
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 用户管理
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="loggerHelper"></param>

        public UserController(IUserServices userService, ILoggerHelper loggerHelper)
        {
            _userService = userService;
            _logger = loggerHelper;
        }

        /// <summary>
        /// 添加登录日志
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("PostUserOpenIdAsync")]
        public async Task<IActionResult> PostWx_UserOpenIdAsync([FromBody]AddUserLog dto)
        {

            try
            {
                var data = await _userService.PostWx_UserOpenIdAsync(dto.code);
                return SuccessMsg("登录成功");

            }
            catch (Exception err)
            {
                _logger.Error(typeof(UserController), "添加用户登录日志失败!", new Exception(err.Message));
                return FailedMsg("添加用户登录日志失败！" + err.Message);
            }

        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("PostUserAsync")]
        public async Task<IActionResult> PostUserAsync([FromBody]AddUserInfoDto input)
        {
            try
            {
                //需要截取Bearer 
                var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var data = await _userService.PostUserAsync(input, tokenHeader);
                return SuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(UserController), "添加用户失败!", new Exception(err.Message));
                return FailedMsg("添加用户失败！" + err.Message);
            }
            
        }

        /// <summary>
        /// 查询用户是否存在
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetUserInfoExists")]
        public async Task<IActionResult> GetUserInfoExists()
        {
            //需要截取Bearer 
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var data = await _userService.GetUserInfoExists(tokenHeader);
            return SuccessData(data);
        }
    }
}
