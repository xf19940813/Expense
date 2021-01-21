using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IRepository
{
    public interface IExpenseRepository : IBaseRepository<ExpenseInfo>
    {
        /// <summary>
        /// 添加报销信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> PostExpenseInfoAsync(AddExpenseInfoDto input, string openId);

        /// <summary>
        /// 我的报销分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="Status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页显示多少条</param>
        /// <returns></returns>
        Task<PageResult<ExpenseInfo>> GetExpenseInfoByStatus(string teamId, short? Status, int pageIndex, int pageSize, string openId);

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> AuditAsync(AuditDto dto, string openId);

        /// <summary>
        /// 完结
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> FinishedAsync(string Id);

        /// <summary>
        /// 报销单详情
        /// </summary>
        /// <param name="ExpenseId">报销单Id</param>
        /// <returns></returns>
        Task<ExpenseInfoDetailDto> GetExpenseDetailByIdAsync(string ExpenseId);

        /// <summary>
        /// 根据报销单ID获取附件信息
        /// </summary>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        Task<List<QuerySheetDto>> GetSheetByExpenseId(string ExpenseId);

    }
}
