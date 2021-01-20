using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dy.Api.Controllers;
using dy.Api.Log;
using dy.Common.Redis;
using dy.IServices;
using dy.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.WeChat
{
    /// <summary>
    /// 设置
    /// </summary>
    public class SettingController : BaseController
    {
        private readonly IRoleServices _roleServices;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="roleServices"></param>
        /// <param name="redisCacheManager"></param>
        /// <param name="loggerHelper"></param>
        public SettingController(IRoleServices roleServices, IRedisCacheManager redisCacheManager, ILoggerHelper loggerHelper)
        {
            _roleServices = roleServices;
            _redisCacheManager = redisCacheManager;
            _logger = loggerHelper;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPost("PostRoleAsync")]
        public async Task<IActionResult> PostRoleAsync([FromBody]AddRoleDto dto)
        {
            string openId = GetOpenId();
            try
            {
                await _roleServices.PostRoleAsync(dto, openId);
                return AddSuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(SettingController), "添加角色失败!", new Exception(err.Message));
                return FailedMsg("添加角色失败！" + err.Message);
            }
        }


        /// <summary>
        /// 修改成员角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateTeamMemberRoleAsync")]
        public async Task<IActionResult> UpdateTeamMemberRoleAsync([FromBody] UpdateRoleDto dto)
        {
            string openId = GetOpenId();
            try
            {
                var data = await _roleServices.UpdateTeamMemberRoleAsync(dto, openId);
                return Ok(data);
            }
            catch(Exception err)
            {
                _logger.Error(typeof(SettingController), "修改角色失败!", new Exception(err.Message));
                return FailedMsg("修改角色失败！" + err.Message);
            }
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetRoleList")]
        public async Task<IActionResult> GetRoleList(string teamId)
        {
            try
            {
                var data = await _roleServices.GetRoleList(teamId);
                return Ok(data);
            }
            catch(Exception err)
            {
                _logger.Error(typeof(SettingController), "获取角色列表失败!", new Exception(err.Message));
                return FailedMsg("获取角色列表失败！" + err.Message);
            }

        }
    }
}
