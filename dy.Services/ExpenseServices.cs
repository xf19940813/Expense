using dy.IRepository;
using dy.IServices;
using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpenseServices : BaseServices<ExpenseInfo>, IExpenseServices
    {
        private readonly IExpenseRepository _expenseDal;

        public ExpenseServices(IBaseRepository<ExpenseInfo> baseRepository, IExpenseRepository expenseDal)
             : base(baseRepository)
        {
            _expenseDal = expenseDal;
        }

        /// <summary>
        /// 添加消费信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> PostExpenseInfoAsync(AddExpenseInfoDto input, string openId)
        {
            return await _expenseDal.PostExpenseInfoAsync(input, openId);
        }

        /// <summary>
        /// 我的报销分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="Status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页显示多少条</param>
        /// <returns></returns>
        public async Task<PageResult<ExpenseInfo>> GetExpenseInfoByStatus(string teamId, short? Status, int pageIndex, int pageSize, string openId)
        {
            return await _expenseDal.GetExpenseInfoByStatus(teamId, Status, pageIndex, pageSize, openId);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> AuditAsync(AuditDto dto, string openId)
        {
            return await _expenseDal.AuditAsync(dto, openId);
        }

        /// <summary>
        /// 完结
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> FinishedAsync(string Id)
        {
            return await _expenseDal.FinishedAsync(Id);
        }

        /// <summary>
        /// 报销单详情
        /// </summary>
        /// <param name="ExpenseId">报销单Id</param>
        /// <returns></returns>
        public async Task<ExpenseInfoDetailDto> GetExpenseDetailByIdAsync(string ExpenseId)
        {
            return await _expenseDal.GetExpenseDetailByIdAsync(ExpenseId);
        }

        /// <summary>
        /// 根据报销单ID获取附件信息
        /// </summary>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        public async Task<List<QuerySheetDto>> GetSheetByExpenseId(string ExpenseId)
        {
            return await _expenseDal.GetSheetByExpenseId(ExpenseId);
        }
    }
}
