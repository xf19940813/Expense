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
    ///  成员管理
    /// </summary>
    public class TeamMemberController : BaseController
    {
        private readonly ITeamMemberServices _memberServices;
        private readonly ILoggerHelper _logger;
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        /// 成员管理
        /// </summary>
        /// <param name="memberServices"></param>
        /// <param name="redisCacheManager"></param>
        /// <param name="loggerHelper"></param>
        public TeamMemberController(ITeamMemberServices memberServices, IRedisCacheManager redisCacheManager, ILoggerHelper loggerHelper)
        {
            _memberServices = memberServices;
            _redisCacheManager = redisCacheManager;
            _logger = loggerHelper;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("PostTeamMemberAsync")]
        public async Task<IActionResult> PostTeamMemberAsync([FromBody]AddTeamMemberDto dto)
        {
            var tokenInfo = GetTokenInfo();
            var openId = tokenInfo.OpenId;
            var sessionKey = tokenInfo.SessionKey;
            try
            {
                var data = await _memberServices.PostTeamMemberAsync(dto, openId, sessionKey);
                return AddSuccessMsg();
            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamMemberController), new Exception(err.Message));
                return FailedMsg(err.Message);
            }
        }

        /// <summary>
        /// 成员列表
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTeamMemberListAsync")]
        public async Task<IActionResult> GetTeamMemberListAsync(string teamId, int pageIndex, int pageSize)
        {
            try
            {
                var data = await _memberServices.GetTeamMemberListAsync(teamId, pageIndex, pageSize);
                return Ok(data);

            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "获取成员列表失败!", new Exception(err.Message));
                return FailedMsg("获取成员列表失败! " + err.Message);
            }
        }

        /// <summary>
        /// 移除成员
        /// </summary>
        /// <param name="Id">成员Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DelTeamMemberByIdAsync")]
        public async Task<IActionResult> DelTeamMemberByIdAsync(string Id)
        {
            try
            {
                await _memberServices.DelTeamMemberByIdAsync(Id);
                return DeleteSuccessMsg();
            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "移除成员失败!", new Exception(err.Message));
                return FailedMsg("移除成员失败! " + err.Message);
            }
        }

        /// <summary>
        /// 修改我在团队显示的昵称
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateTeamNickNameAsync")]
        public async Task<IActionResult> UpdateTeamNickNameAsync([FromBody] UpdateTeamNickNameDto dto)
        {
            try
            {
                await _memberServices.UpdateTeamNickNameAsync(dto);
                return UpdateSuccessMsg();
            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "更新失败!", new Exception(err.Message));
                return FailedMsg("更新失败! " + err.Message);
            }
        }

        /// <summary>
        /// 当前成员角色
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTeamMemberRoleAsync")]
        public async Task<IActionResult> GetTeamMemberRoleAsync(string teamId)
        {
            string openId = GetOpenId();
            try
            {
                var data = await _memberServices.GetTeamMemberRoleAsync(teamId, openId);
                return SuccessData(data);
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "获取成员角色失败!", new Exception(err.Message));
                return FailedMsg("获取成员角色失败! " + err.Message);
            }
        }

        /// <summary>
        /// 成员额度分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="memberId">成员Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetMemberQuotaAsync")]
        public async Task<IActionResult> GetMemberQuotaAsync(string teamId, string memberId, int pageIndex, int pageSize)
        {
            try
            {
                var data = await _memberServices.GetMemberQuotaAsync(teamId, memberId, pageIndex, pageSize);
                return Ok(data);
            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "获取成员额度失败!", new Exception(err.Message));
                return FailedMsg("获取成员额度失败! " + err.Message);
            }
        }

        /// <summary>
        /// 修改成员额度
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateMemberQuotaAsync")]
        public async Task<IActionResult> UpdateMemberQuotaAsync([FromBody]UpdateMemberQuotaDto dto)
        {
            try
            {
                await _memberServices.UpdateMemberQuotaAsync(dto);
                return UpdateSuccessMsg();
            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "更新失败!", new Exception(err.Message));
                return FailedMsg("更新失败! " + err.Message);
            }
        }

        /// <summary>
        /// 获取当前成员在团队的昵称
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTeamNickNameAsync")]
        public async Task<IActionResult> GetTeamNickNameAsync(string teamId)
        {
            string openId = GetOpenId();
            try
            {
                var data = await _memberServices.GetTeamNickNameAsync(teamId, openId);
                return SuccessData(data);
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamMemberController), "获取昵称失败!", new Exception(err.Message));
                return FailedMsg("获取昵称失败! " + err.Message);
            }
        }
    }
}
