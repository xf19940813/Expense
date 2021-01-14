using AutoMapper;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IRepository;
using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using dy.Model.User;
using Microsoft.AspNetCore.Authorization;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Repository
{
    public class ExpenseRepository : BaseRepository<ExpenseInfo>, IExpenseRepository
    {
        private readonly IMapper iMapper;
        private readonly IRedisCacheManager _redisCacheManager;
        public ExpenseRepository(IMapper IMapper, IRedisCacheManager redisCacheManager)
        {
            iMapper = IMapper;
            _redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 添加报销信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> PostExpenseInfoAsync(AddExpenseInfoDto input, string openId)
        {
            if(input.TeamId == null) throw new Exception("团队Id不能为空！");

            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID; //找到UserId

            var FreeQuota = db.Queryable<TeamMember>().Where(a => a.TeamId == input.TeamId && a.CreateUserId == UserId).First()?.FreeQuota;

            ExpenseInfo expense = iMapper.Map<ExpenseInfo>(input);
            expense.ID = IdHelper.CreateGuid();
            expense.CreateUserId = UserId;
            expense.IsDeleted = false;

            if (input.Amount > FreeQuota)
                expense.AuditStatus = AppConsts.AuditStatus.UnAudited;
            else
                expense.AuditStatus = AppConsts.AuditStatus.Audited;

            return await Task.Run(() =>
            {
                var result = db.Insertable(expense).ExecuteCommand();
                return result > 0;
            });
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
            if (teamId == null) throw new Exception("团队Id为空！");

            return await Task.Run(() =>
            {
                PageResult<ExpenseInfo> pageResult = new PageResult<ExpenseInfo>();

                var data = db.Queryable<ExpenseInfo, TeamMember>((a, b) => new JoinQueryInfos
                (
                    JoinType.Inner, a.TeamId == b.TeamId && a.CreateUserId == b.CreateUserId
                ))
                .WhereIF(Status>=0, (a, b) => a.AuditStatus == Status)
                .Where((a, b) => a.TeamId == teamId).OrderBy("a.CreateTime desc")
                .ToPageList(pageIndex, pageSize);
                pageResult.totalCount = entityDB.AsQueryable().Where(a => a.IsDeleted == false).Count();
                pageResult.pageIndex = pageIndex;
                pageResult.pageSize = pageSize;
                pageResult.data = data;

                return pageResult;
            });
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> AuditAsync(string Id)
        {
            return await Task.Run(() =>
            {
                var result = db.Updateable<ExpenseInfo>(a => a.AuditStatus == AppConsts.AuditStatus.Audited).Where(a => a.ID == Id).ExecuteCommand();

                return result > 0;
            });

        }

        /// <summary>
        /// 完结
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> FinishedAsync(string Id)
        {
            return await Task.Run(() =>
            {
                var result = db.Updateable<ExpenseInfo>(a => a.AuditStatus == AppConsts.AuditStatus.Finished).Where(a => a.ID == Id).ExecuteCommand();

                return result > 0;
            });
        }
    }
}
