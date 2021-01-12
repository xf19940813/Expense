using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dy.Api.Controllers;
using dy.Api.Log;
using dy.IServices;
using dy.Model.Dto;
using dy.Model.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.WeChat
{
    /// <summary>
    /// 报销管理
    /// </summary>
    public class ExpenseController : BaseController
    {
        private readonly IExpenseServices _expenseServices;
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 报销管理
        /// </summary>
        /// <param name="expenseServices"></param>
        /// <param name="loggerHelper"></param>
        public ExpenseController(IExpenseServices expenseServices, ILoggerHelper loggerHelper)
        {
            _expenseServices = expenseServices;
            _logger = loggerHelper;
        }

        /// <summary>
        /// 添加消费信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("PostExpenseInfoAsync")]
        public async Task<IActionResult> PostExpenseInfoAsync([FromBody]AddExpenseInfoDto input)
        {
            //从Header中获取Token
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                var data = await _expenseServices.PostExpenseInfoAsync(input, tokenHeader);
                return AddSuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(ExpenseController), "添加消费信息失败!", new Exception(err.Message));
                return FailedMsg("添加消费信息失败！" + err.Message);
            }
        }

        /// <summary>
        /// 我的报销分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="Status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页显示多少条</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetExpenseInfoAsync")]
        public async Task<IActionResult> GetExpenseInfoAsync(string teamId,short? Status, int pageIndex, int pageSize)
        {
            try
            {
                var data = await _expenseServices.GetExpenseInfoByStatus(teamId, Status, pageIndex, pageSize);
                return Ok(data);

            }
            catch(Exception err)
            {

                _logger.Error(typeof(TeamController), "获取报销列表失败!", new Exception(err.Message));
                return FailedMsg("获取报销列表失败! " + err.Message);
            }
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="Id">报销信息Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("AuditAsync")]
        public async Task<IActionResult> AuditAsync([FromBody]string Id)
        {
            try
            {
                var data = await _expenseServices.AuditAsync(Id);
                return AuditSuccessMsg();
            }
            catch (Exception err)
            {
                _logger.Error(typeof(ExpenseController), "审核失败!", new Exception(err.Message));
                return FailedMsg("审核失败！" + err.Message);
            }
        }

        /// <summary>
        /// 报销完结
        /// </summary>
        /// <param name="Id">报销信息Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("FinishedAsync")]
        public async Task<IActionResult> FinishedAsync([FromBody]string Id)
        {
            try
            {
                var data = await _expenseServices.FinishedAsync(Id);
                return SuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error(typeof(ExpenseController), "报销失败", new Exception(err.Message));
                return FailedMsg(err.Message);
            }
        }
    }
}
