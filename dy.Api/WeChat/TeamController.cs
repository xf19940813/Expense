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
    /// 团队管理
    /// </summary>
    public class TeamController : BaseController
    {
        private readonly ITeamServices _teamServices;
        private readonly ILoggerHelper _logger;
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        /// 团队管理
        /// </summary>
        /// <param name="teamServices"></param>
        /// <param name="redisCacheManager"></param>
        /// <param name="loggerHelper"></param>
        public TeamController(ITeamServices teamServices, IRedisCacheManager redisCacheManager, ILoggerHelper loggerHelper)
        {
            _teamServices = teamServices;
            _redisCacheManager = redisCacheManager;
            _logger = loggerHelper;
        }

        /// <summary>
        /// 添加团队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("PostTeamAsync")]
        public async Task<IActionResult> PostTeamAsync([FromBody] AddTeamDto input)
        {
            //从Header中获取Token
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                var data = await _teamServices.PostTeamAsync(input, tokenHeader);
                return SuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamController), "添加团队失败!", new Exception(err.Message));
                return FailedMsg("添加团队失败！" + err.Message);
            }

        }

        /// <summary>
        /// 团队列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTeamListAsync")]
        public async Task<IActionResult> GetTeamListAsync(int pageIndex, int pageSize)
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

            try
            {
                var data = await _teamServices.GetTeamListAsync(pageIndex, pageSize, openId);
                return Ok(data);

            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamController), "获取团队列表失败!", new Exception(err.Message));
                return FailedMsg("获取团队列表失败! " + err.Message);
            }
        }

        /// <summary>
        /// 修改团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateTeamAsync")]
        public async Task<IActionResult> UpdateTeamAsync([FromBody]UpdateTeamDto dto)
        {
            //从Header中获取Token
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isKey = _redisCacheManager.Get(tokenHeader);
            string openId = string.Empty;
            if (isKey)
            {
                openId = _redisCacheManager.GetValue(tokenHeader).ToString().Split(";")[0].Trim('"');
            }

            try
            {
                var data = await _teamServices.UpdateTeamAsync(dto, openId);
                return UpdateSuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamController), "更新团队信息失败!", new Exception(err.Message));
                return FailedMsg("更新团队信息失败! " + err.Message);
            }
        }

        /// <summary>
        /// 移除团队
        /// </summary>
        /// <param name="Id">团队Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteByIdAsync")]
        public async Task<IActionResult> DeleteByIdAsync(string Id)
        {
            try
            {
                await _teamServices.DeleteByIdAsync(Id);
                return DeleteSuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamController), "删除团队失败!", new Exception(err.Message));
                return FailedMsg("删除团队失败! " + err.Message);
            }
        }

        /// <summary>
        /// 获取团队Id获取团队信息
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTeamByIdAsync")]
        public async Task<IActionResult> GetTeamByIdAsync(string teamId)
        {
            try
            {
                var data = await _teamServices.GetTeamByIdAsync(teamId);
                return SuccessData(data);

            }
            catch (Exception err)
            {
                _logger.Error(typeof(TeamController), "获取团队信息失败!", new Exception(err.Message));
                return FailedMsg("获取团队信息失败! " + err.Message);
            }
        }

        /// <summary>
        /// 团队转让
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>、
        [Authorize]
        [HttpPut("TransferTeamAsync")]
        public async Task<IActionResult> TransferTeamAsync([FromBody] TransferTeamDto dto)
        {
            //从Header中获取Token
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isKey = _redisCacheManager.Get(tokenHeader);
            string openId = string.Empty;
            if (isKey)
            {
                openId = _redisCacheManager.GetValue(tokenHeader).ToString().Split(";")[0].Trim('"');
            }

            try
            {
                var data = await _teamServices.TransferTeamAsync(dto, openId);

                return SuccessMsg("转让成功！");
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamController), "转让失败！",new Exception(err.Message));
                return FailedMsg(err.Message);
            }
            
        }

        /// <summary>
        /// 团队启用/禁用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("IsEnabledAsync")]
        public async Task<IActionResult> IsEnabledAsync([FromBody] IsEnabledDto dto)
        {
            try
            {
                await _teamServices.IsEnabledAsync(dto);
                return UpdateSuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(TeamController), "更新失败！", new Exception(err.Message));
                return FailedMsg(err.Message);
            }
        }
    }
}
