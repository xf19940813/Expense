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
        public async Task<bool> PostExpenseInfoAsync(AddExpenseInfoDto input, string tokenHeader)
        {
            return await _expenseDal.PostExpenseInfoAsync(input, tokenHeader);
        }

        /// <summary>
        /// 我的报销分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="Status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页显示多少条</param>
        /// <returns></returns>
        public async Task<PageResult<ExpenseInfo>> GetExpenseInfoByStatus(string teamId, short? Status, int pageIndex, int pageSize)
        {
            return await _expenseDal.GetExpenseInfoByStatus(teamId, Status, pageIndex, pageSize);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> AuditAsync(string Id)
        {
            return await _expenseDal.AuditAsync(Id);
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
    }
}
