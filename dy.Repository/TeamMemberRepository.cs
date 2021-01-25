using AutoMapper;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IRepository;
using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using dy.Model.Setting;
using dy.Model.User;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Repository
{
    public class TeamMemberRepository : BaseRepository<TeamMember>, ITeamMemberRepository
    {
        private readonly IMapper iMapper;
        private readonly IRedisCacheManager _redisCacheManager;

        public TeamMemberRepository(IMapper IMapper, IRedisCacheManager redisCacheManager)
        {
            iMapper = IMapper;
            _redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> PostTeamMemberAsync(AddTeamMemberDto dto, string openId)
        {

            var isAny = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).Any();
            var result = 0;
            return await Task.Run(() =>
            {
                if (isAny == false)
                {
                    Wx_UserInfo userInfo = iMapper.Map<Wx_UserInfo>(dto);
                    userInfo.ID = IdHelper.CreateGuid();
                    userInfo.OpenId = openId;
                    db.Insertable(userInfo).ExecuteCommand();
                }

                //获取邀请人的UserId
                bool isInviterToken = _redisCacheManager.Get(dto.InviterToken);
                string InviterUserId = string.Empty;
                if (isInviterToken)
                {
                    //根据邀请人Token获取到邀请人OpenId
                    var InviterOenId = _redisCacheManager.GetValue(dto.InviterToken).ToString().Split(";")[0].Trim('"');

                    InviterUserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == InviterOenId).First()?.ID;
                }

                var UserInfo = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).Select(a => new
                {
                    UserId = a.ID,
                    MobilePhone = a.MobilePhone

                }).First(); //找到加入人UserId、MobilePhone

                var roleId = db.Queryable<Role>().Where(a => a.TeamId == dto.TeamId && a.Name == AppConsts.RoleName.Ordinary).First()?.ID;

                var isMember = db.Queryable<TeamMember>().Where(a => a.TeamId == dto.TeamId && a.IsDeleted == false && a.JoinedUserId == UserInfo.UserId).Any();

                if (isMember)
                {
                    throw new Exception("成员已经加了团队，不能再次加入！");
                }
                else
                {
                    TeamMember teamMember = iMapper.Map<TeamMember>(dto);
                    teamMember.ID = IdHelper.CreateGuid();
                    teamMember.TeamNickName = dto.NickName;
                    teamMember.IsDeleted = false;
                    teamMember.InviterUserId = InviterUserId;
                    teamMember.JoinedUserId = UserInfo.UserId;
                    teamMember.RoleId = roleId;
                    result = db.Insertable(teamMember).ExecuteCommand();

                    if(result > 0 && UserInfo.MobilePhone != null)
                    {
                        db.Updateable<TeamMember>().SetColumns(a => new TeamMember() { MobilePhone = UserInfo.MobilePhone })
                            .Where(a => a.ID == teamMember.ID).ExecuteCommand();
                    }
                    
                }

                return result > 0;
            });
        }

        /// <summary>
        /// 分页获取成员数据
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        public async Task<PageResult<QueryTeamMemberDto>> GetTeamMemberListAsync(string teamId, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                PageResult<QueryTeamMemberDto> pageResult = new PageResult<QueryTeamMemberDto>();

                var data = db.Queryable<Team, TeamMember, Role>((a, b, c) => new JoinQueryInfos(
                       JoinType.Inner, a.ID == b.TeamId,
                       JoinType.Inner, c.ID == b.RoleId
                ))
                 .Where(a => a.IsDeleted == false)
                 .Where((a, b, c) => b.IsDeleted == false && b.TeamId == teamId)
                 .Select((a, b, c) => new QueryTeamMemberDto
                  {
                        ID = b.ID,
                        TeamNickName = b.TeamNickName,
                        Headimgurl = b.Headimgurl,
                        JoinedTime = b.JoinedTime,
                        RoleName = c.Name
                  })
                .OrderBy("b.JoinedTime desc")

                .ToPageList(pageIndex, pageSize);

                pageResult.data = data;
                pageResult.pageIndex = pageIndex;
                pageResult.pageSize = pageSize;
                pageResult.totalCount = entityDB.AsQueryable().Where(a => a.IsDeleted == false).Count();

                return pageResult;
            });
        }

        /// <summary>
        /// 根据成员Id移除成员
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> DelTeamMemberByIdAsync(string Id)
        {
            return await Task.Run(() =>
            {
                var i = db.Updateable<TeamMember>(a => a.IsDeleted == true).Where(a => a.ID == Id).ExecuteCommand();

                return i > 0;
            });
        }

        /// <summary>
        /// 修改我在团队显示的昵称
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamNickNameAsync(UpdateTeamNickNameDto dto)
        {
            if (dto.MemberId == null) throw new Exception("成员Id为空！");
            if (dto.TeamId == null) throw new Exception("团队Id为空！");
            return await Task.Run(() =>
            {
                var i = db.Updateable<TeamMember>(a => a.TeamNickName == dto.TeamNickName).Where(a => a.ID == dto.MemberId && a.TeamId == dto.TeamId).ExecuteCommand();

                return i > 0;
            });
        }

        /// <summary>
        /// 根据成员Id查询当前成员的角色
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<string> GetTeamMemberRoleAsync(string teamId, string openId)
        {
            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID; //找到UserId

            return await Task.Run(() =>
            {
                var roleId = db.Queryable<TeamMember>().Where(a => a.TeamId == teamId && a.JoinedUserId == UserId).First()?.RoleId;
                var roleName = db.Queryable<Role>().Where(a => a.ID == roleId).First()?.Name;

                return roleName;
            });
            
        }

        /// <summary>
        /// 成员额度分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="memberId">成员Id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResult<QueryMemberQuotaDto>> GetMemberQuotaAsync(string teamId, string memberId, int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                PageResult<QueryMemberQuotaDto> pageResult = new PageResult<QueryMemberQuotaDto>();

                if(memberId != null)
                {
                    pageIndex = 1;
                    pageSize = 1;
                }

                var data = db.Queryable<TeamMember>().WhereIF(memberId != null, a => a.ID == memberId)
                .Where(a => a.TeamId == teamId)
                .Select(a => new QueryMemberQuotaDto
                { 
                    MemberId = a.ID,
                    TeamNickName = a.TeamNickName,
                    Headimgurl = a.Headimgurl,
                    FreeQuota = a.FreeQuota
                }).OrderBy("JoinedTime desc")
                .ToPageList(pageIndex, pageSize);

                pageResult.data = data;
                pageResult.pageIndex = pageIndex;
                pageResult.pageSize = pageSize;
                pageResult.totalCount = entityDB.AsQueryable().Count();

                return pageResult;
            });
        }

        /// <summary>
        /// 修改成员额度
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMemberQuotaAsync(UpdateMemberQuotaDto dto)
        {
            return await Task.Run(() =>
            {
                var i = db.Updateable<TeamMember>(a => a.FreeQuota == dto.FreeQuota).Where(a => a.ID == dto.MemberId).ExecuteCommand();

                return i > 0;
            });
        }

        /// <summary>
        /// 获取当前成员在团队的昵称
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<GetTeamNickNameDto> GetTeamNickNameAsync(string teamId, string openId)
        {
            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID; //找到UserId

            return await Task.Run(() =>
            {
                var query = db.Queryable<TeamMember>().Where(a => a.TeamId == teamId && a.JoinedUserId == UserId)
                .Select(a => new GetTeamNickNameDto
                { 
                    MemberId = a.ID,
                    TeamNickName = a.TeamNickName
                });
                return query.First();
            });
        }
    }
}
