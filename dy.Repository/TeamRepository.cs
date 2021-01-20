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
    /// <summary>
    /// 
    /// </summary>
    public class TeamRepository : BaseRepository<Team>, ITeamRepository
    {
        private readonly IMapper iMapper;
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IMapper"></param>
        /// <param name="redisCacheManager"></param>
        public TeamRepository(IMapper IMapper, IRedisCacheManager redisCacheManager)
        {
            iMapper = IMapper;
            _redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 添加团队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> PostTeamAsync(AddTeamDto input, string openId)
        {
            var query = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId)
                .Select(a => new QueryUserInfoDto 
                { 
                    UserId = a.ID,
                    NickName = a.NickName,
                    Headimgurl = a.Headimgurl,
                    MobilePhone = a.MobilePhone
                }).First();

            Team team = iMapper.Map<Team>(input); //团队
            Role role = new Role(); //角色
            TeamMember teamMember = new TeamMember(); //成员

            var result = 0;
            return await Task.Run(() =>
            {
                db.BeginTran();//开启事务

                var isTeam = db.Queryable<Team>()
                .Where(a => a.IsDeleted == false && a.TeamName == input.TeamName.Trim()).Any();

                if (isTeam)
                    throw new Exception("该名称已被使用！");

                if (query != null)
                {
                    //添加团队
                    team.ID = IdHelper.CreateGuid();
                    team.CreateUserId = query.UserId;
                    team.IsDeleted = false;
                    team.IsEnabled = true;
                    db.Insertable(team).ExecuteCommand();

                    //添加角色
                    role.ID = IdHelper.CreateGuid();
                    role.Name = AppConsts.RoleName.Creator;
                    role.CreatorUserId = query.UserId;
                    role.TeamId = team.ID;
                    role.IsDeleted = false;
                    role.IsEnabled = true;
                    db.Insertable(role).ExecuteCommand();

                    //添加成员
                    teamMember.ID = IdHelper.CreateGuid();
                    teamMember.NickName = query.NickName;
                    teamMember.TeamNickName = query.NickName;
                    teamMember.Headimgurl = query.Headimgurl;
                    teamMember.MobilePhone = query.MobilePhone;
                    teamMember.TeamId = team.ID;
                    teamMember.RoleId = role.ID;
                    teamMember.JoinedUserId = query.UserId;
                    teamMember.IsDeleted = false;
                    db.Insertable(teamMember).ExecuteCommand();

                    List<Role> roles = new List<Role>() {
                        new Role()
                        {
                             ID = IdHelper.CreateGuid(),
                             Name = AppConsts.RoleName.Admin,  //管理员
                             CreatorUserId = query.UserId,
                             TeamId = team.ID,
                             IsDeleted = false,
                             IsEnabled = true
                        } ,
                        new Role(){
                             ID = IdHelper.CreateGuid(),
                             Name = AppConsts.RoleName.Finance, //财务
                             CreatorUserId = query.UserId,
                             TeamId = team.ID,
                             IsDeleted = false,
                             IsEnabled = true
                        },
                        new Role(){
                             ID = IdHelper.CreateGuid(),
                             Name = AppConsts.RoleName.Ordinary, //普通员工
                             CreatorUserId = query.UserId,
                             TeamId = team.ID,
                             IsDeleted = false,
                             IsEnabled = true
                        }
                    };
                    result = db.Insertable(roles).ExecuteCommand();
                }

                db.CommitTran();

                return result > 0;
            });
        }

        /// <summary>
        /// 团队列表分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <param name="openId">用户唯一标识</param>
        /// <returns></returns>
        public async Task<PageResult<QueryTeamDto>> GetTeamListAsync(int pageIndex, int pageSize, string openId)
        {
            return await Task.Run(() =>
            {
                var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID;

                PageResult<QueryTeamDto> pageResult = new PageResult<QueryTeamDto>();

                var data = db.Queryable<Team, TeamMember>((a, b) => new JoinQueryInfos(
                     JoinType.Inner, a.ID == b.TeamId
                ))
                .Where((a, b)=> a.IsDeleted == false)
                .Where((a, b) => b.JoinedUserId == UserId)
                .Where((a, b) => b.IsDeleted == false)
                .Select(a => new QueryTeamDto
                { 
                    ID = a.ID,
                    TeamName = a.TeamName,
                    IsEnabled = a.IsEnabled
                })
                .OrderBy("a.CreateTime desc")
                .ToPageList(pageIndex, pageSize);

                pageResult.data = data;
                pageResult.pageIndex = pageIndex;
                pageResult.pageSize = pageSize;
                pageResult.totalCount = entityDB.AsQueryable().Where(a => a.IsDeleted == false).Count();

                return pageResult;
            });
        }


        /// <summary>
        /// 更新团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamAsync(UpdateTeamDto dto, string openId)
        {
            return await Task.Run(() =>
            {
                var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID;

                var result = db.Updateable<Team>().SetColumns(a => new Team() {
                    TeamName = dto.TeamName,
                    TeamInfo = dto.TeamInfo,
                    LastModifyUserId = UserId,
                    LastModifyTime = DateTime.Now
                })
                .Where(a => a.ID == dto.ID)
                .ExecuteCommand();

                return result > 0;
            });
        }


        /// <summary>
        /// 根据Id删除团队
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdAsync(string Id, string openId)
        {
            if (Id == null) throw new Exception("团队Id为空！");
            if (openId == null) throw new Exception("用户唯一标识为空！");

            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID;

            var Role = db.Queryable<TeamMember, Role>((a, b) => new JoinQueryInfos
            (
                JoinType.Inner, a.RoleId == b.ID
            )).Where((a, b) => a.TeamId == Id && a.JoinedUserId == UserId)
            .Select((a, b) => new
            { 
                RoleName = b.Name
            }).First();

            if (Role.RoleName != AppConsts.RoleName.Creator)
                throw new Exception("您不是团队拥有者，无此权限！");

            return await Task.Run(() =>
            {
                var i = db.Updateable<Team>(a => a.IsDeleted == true).Where(a => a.ID == Id).ExecuteCommand();

                return i > 0;
            });  
        }

        /// <summary>
        /// 根据团队Id获取团队信息
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<GetTeamDto> GetTeamByIdAsync(string teamId)
        {
            return await Task.Run(() =>
            {
                var query = db.Queryable<Team>().Where(a => a.IsDeleted == false && a.ID == teamId)
                .Select(a => new GetTeamDto
                {
                    ID = a.ID,
                    TeamName = a.TeamName,
                    TeamInfo = a.TeamInfo
                });
                return query.FirstAsync();

            });
        }

        /// <summary>
        /// 团队转让
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> TransferTeamAsync(TransferTeamDto dto, string openId)
        {
            if(dto.TeamId == null) throw new Exception ("团队Id为空！");
            if (dto.MemberId == null) throw new Exception("成员Id为空!");
            if (openId == null) throw new Exception("用户唯一标识为空！");

            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID; //找到UserId

            var Role = db.Queryable<TeamMember, Role>((a, b) => new JoinQueryInfos
            (
                JoinType.Inner, a.RoleId == b.ID
            )).Where((a, b) => a.TeamId == dto.TeamId && a.JoinedUserId == UserId)
            .Select((a, b) => new
            {
                RoleName = b.Name
            }).First();

            if (Role.RoleName != AppConsts.RoleName.Creator)
                throw new Exception("您不是团队拥有者，无此权限！");

            Role role = new Role();
            var result = 0;

            string CreatorRoleId = string.Empty; //创建者角色Id
            string AdminRoleId = string.Empty; //管理角色Id
            return await Task.Run(() =>
            {
                db.BeginTran();

                //找到转让人Id
                var transId = db.Queryable<TeamMember>().Where(t => t.TeamId == dto.TeamId && t.JoinedUserId == UserId).First()?.ID;

                AdminRoleId = db.Queryable<Role>().Where(r => r.TeamId == dto.TeamId && r.Name == AppConsts.RoleName.Admin).First()?.ID;

                CreatorRoleId = db.Queryable<Role>().Where(r => r.TeamId == dto.TeamId && r.Name == AppConsts.RoleName.Creator).First()?.ID;

                //团队转让前先把原有的创建者角色修改为管理员
                db.Updateable<TeamMember>().SetColumns(a => new TeamMember()
                {
                    RoleId = AdminRoleId
                })
                .Where(t => t.ID == transId)
                .ExecuteCommand();

                //更新团队创建者
                db.Updateable<Team>().SetColumns(a => new Team()
                {
                    LastModifyUserId = UserId,
                    LastModifyTime = DateTime.Now
                })
                .Where(a => a.ID == dto.TeamId)
                .ExecuteCommand();

                //更新成员角色、修改时间
                result = db.Updateable<TeamMember>().SetColumns(a => new TeamMember()
                {
                    RoleId = CreatorRoleId,
                    LastModifyTime = DateTime.Now
                })
                .Where(a => a.ID == dto.MemberId)
                .ExecuteCommand();

                db.CommitTran();

                return result > 0;

            });
        }

        /// <summary>
        /// 团队启用/禁用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> IsEnabledAsync(IsEnabledDto dto)
        {
            bool isEnabled = false;
            bool.TryParse(dto.Value, out isEnabled);

            return await Task.Run(() =>
            {
                var result = db.Updateable<Team>().SetColumns(a => new Team()
                {
                    IsEnabled = isEnabled
                })
                .Where(a => a.ID == dto.TeamId)
                .ExecuteCommand();

                return result > 0;
            });
        }
    }
}
