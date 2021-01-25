using AutoMapper;
using dy.Common.Config;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IRepository;
using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using dy.Model.Setting;
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
        public async Task<string> PostExpenseInfoAsync(AddExpenseInfoDto input, string openId)
        {
            if(input.TeamId == null) throw new Exception("团队Id不能为空！");

            var isEnabled = db.Queryable<Team>().Where(a => a.ID == input.TeamId).First()?.IsEnabled;
            if (isEnabled == false)
                throw new Exception("该团队处于禁用状态，不能申请报销!");

            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID; //找到UserId

            var FreeQuota = db.Queryable<TeamMember>().Where(a => a.TeamId == input.TeamId && a.JoinedUserId == UserId).First()?.FreeQuota;

            ExpenseInfo expense = iMapper.Map<ExpenseInfo>(input);
            expense.ID = IdHelper.CreateGuid();
            expense.CreateUserId = UserId;
            expense.IsDeleted = false;

            if (input.Amount > FreeQuota)
                expense.AuditStatus = AppConsts.AuditStatus.UnAudited;
            else
                expense.AuditStatus = AppConsts.AuditStatus.Audited;

            var result = 0;
            return await Task.Run(() =>
            {
                Sheet sheet = new Sheet(); // 附件

                if (!string.IsNullOrEmpty(input.ImgNames))
                {
                    string[] imgArray = input.ImgNames.Split(','); //字符串转数组
                    foreach (var img in imgArray)
                    {
                        sheet.ID = IdHelper.CreateGuid();
                        sheet.ExpenseId = expense.ID;
                        sheet.TeamId = expense.TeamId;
                        sheet.ImgUrl = ImgConfig.img_url + img;
                        sheet.IsDeleted = false;
                        db.Insertable(sheet).ExecuteCommand();
                    }
                }

                result = db.Insertable(expense).ExecuteCommand();
                
                if (result <= 0) throw new Exception("添加失败");

                return expense.ID;
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
        public async Task<PageResult<ExpenseInfo>> GetExpenseInfoByStatus(string teamId, short? Status, int pageIndex, int pageSize, string openId)
        {
            if (teamId == null) throw new Exception("团队Id为空！");

            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First().ID;

            var Role = db.Queryable<TeamMember, Role>((t, r) => new JoinQueryInfos
            (
                JoinType.Inner, t.RoleId == r.ID && t.TeamId == r.TeamId
            ))
            .Where((t,r) => t.TeamId == teamId)
            .Where((t, r) => t.JoinedUserId == UserId)
            .Select((t, r) => new QueryRoleDto
            {
                RoleId = t.RoleId,
                RoleName = r.Name
            }).First();

            return await Task.Run(() =>
            {
                PageResult<ExpenseInfo> pageResult = new PageResult<ExpenseInfo>();

                var data = db.Queryable<ExpenseInfo, TeamMember>((a, b) => new JoinQueryInfos
                (
                    JoinType.Inner, a.TeamId == b.TeamId && a.CreateUserId == b.JoinedUserId
                ))
                .WhereIF(Status>=0, (a, b) => a.AuditStatus == Status)
                .WhereIF(Role.RoleName.Trim() == AppConsts.RoleName.Ordinary, (a, b) => a.CreateUserId == UserId)
                .Where((a, b) => a.TeamId == teamId)
                .Where((a, b) => b.IsDeleted == false)
                .OrderBy((a, b) => a.CreateTime, OrderByType.Desc)
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
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> AuditAsync(AuditDto dto, string openId)
        {
            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID;

            return await Task.Run(() =>
            {
                var result = db.Updateable<ExpenseInfo>()
                .SetColumns(a => new ExpenseInfo() { AuditStatus = AppConsts.AuditStatus.Audited, AuditUserId = UserId, AuditTime = DateTime.Now })
                .Where(a => a.ID == dto.Id).ExecuteCommand();

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

        /// <summary>
        /// 报销单详情
        /// </summary>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        public async Task<ExpenseInfoDetailDto> GetExpenseDetailByIdAsync(string ExpenseId)
        {
            return await Task.Run(() =>
            {
                var query = db.Queryable<ExpenseInfo>().Where(a => a.ID == ExpenseId && a.IsDeleted == false).Select(a => new
                {
                    TeamId = a.TeamId,
                    CreateUserId = a.CreateUserId,
                    AuditUserId = a.AuditUserId
                }).First();

                var result = db.Queryable<ExpenseInfo, TeamMember>((a, b) => new JoinQueryInfos
                (
                    JoinType.Inner, a.TeamId == b.TeamId && a.CreateUserId == b.JoinedUserId
                ))
                .Where(a => a.ID == ExpenseId)
                .Where((a, b) => b.IsDeleted == false)
                .Select((a, b) => new ExpenseInfoDetailDto
                {
                    
                    AuditStatus = a.AuditStatus,
                    MobilePhone = b.MobilePhone,
                    ConsumeProject = a.ConsumeProject,
                    ExpenseType = a.ExpenseType,
                    ConsumeTime = a.ConsumeTime,
                    ApplyTime = a.CreateTime,
                    AuditTime = a.AuditTime,
                    Remarks = a.Remarks,
                    RejectReason = a.RejectReason

                }).First();

                if (query.CreateUserId != null)
                    result.Proposer = db.Queryable<TeamMember>().Where(a => a.JoinedUserId == query.CreateUserId && a.TeamId == query.TeamId).First().TeamNickName;
                    
                if (query.AuditUserId != null)
                    result.Auditor = db.Queryable<TeamMember>().Where(a => a.JoinedUserId == query.AuditUserId && a.TeamId == query.TeamId).First().TeamNickName;

                return result;
            });
        }

        /// <summary>
        /// 根据报销单ID获取附件信息
        /// </summary>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        public async Task<List<QuerySheetDto>> GetSheetByExpenseId(string ExpenseId)
        {
            return await Task.Run(() =>
            {
                var query = db.Queryable<Sheet>()
                .Where(a => a.IsDeleted == false && a.ExpenseId == ExpenseId)
                .Select(a => new QuerySheetDto
                { 
                    ID = a.ID,
                    ImgUrl = a.ImgUrl
                });

                return query.ToList();
            });
            
        }


        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> RejectAsync(RejectDto dto, string openId)
        {
            var Status = db.Queryable<ExpenseInfo>().Where(a => a.ID == dto.Id).First()?.AuditStatus;

            if (Status != AppConsts.AuditStatus.UnAudited)
                throw new Exception("报销单未审核的情况下才可以驳回！");

            return await Task.Run(() =>
            {
                var userId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID;

                var result = db.Updateable<ExpenseInfo>()
                .SetColumns(a => new ExpenseInfo() { AuditStatus = AppConsts.AuditStatus.Reject, RejectUserId = userId, RejectReason = dto.RejectReason, RejectTime = DateTime.Now })
                .Where(a => a.ID == dto.Id).ExecuteCommand();

                return result > 0;
            });
        }
    }
}
