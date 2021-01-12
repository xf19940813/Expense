﻿using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IServices
{
    public interface IExpenseServices : IBaseServices<ExpenseInfo>
    {
        /// <summary>
        /// 添加报销信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> PostExpenseInfoAsync(AddExpenseInfoDto input, string tokenHeader);

        /// <summary>
        /// 我的报销分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="Status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页显示多少条</param>
        /// <returns></returns>
        Task<PageResult<ExpenseInfo>> GetExpenseInfoByStatus(string teamId, short? Status, int pageIndex, int pageSize);

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> AuditAsync(string Id);

        /// <summary>
        /// 完结
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> FinishedAsync(string Id);
    }
}
