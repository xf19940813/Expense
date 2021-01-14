using AutoMapper;
using dy.Common.Helper;
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
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly IMapper iMapper;

        public RoleRepository(IMapper IMapper)
        {
            iMapper = IMapper;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> PostRoleAsync(AddRoleDto dto, string openId)
        {
            if (dto.TeamId == null) throw new Exception("团队Id为空！");

            var UserId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID; //找到UserId

            Role role = iMapper.Map<Role>(dto);
            role.ID = IdHelper.CreateGuid();
            role.CreatorUserId = UserId;
            role.IsDeleted = false;
            role.IsEnabled = true;

            return await Task.Run(() =>
            {
                var result = db.Insertable(role).ExecuteCommand();

                return result > 0;
            });
        }

        /// <summary>
        ///  修改成员角色
        /// </summary>
        /// <param name="dto"></param>

        /// <returns></returns>
        public async Task<QueryRoleDto> UpdateTeamMemberRoleAsync(UpdateRoleDto dto)
        {
            return await Task.Run(() =>
            {
                var result = db.Updateable<TeamMember>().SetColumns(a => new TeamMember() { 
                    RoleId = dto.RoleId, 
                    LastModifyTime = DateTime.Now 
                })
                .Where(a => a.ID == dto.MemberId && dto.TeamId == dto.TeamId).ExecuteCommand();

                var role = db.Queryable<Role>().Where(a => a.ID == dto.RoleId).Select(a => new QueryRoleDto
                { 
                   RoleId = a.ID,
                   RoleName = a.Name
                }).First();

                return role;
            });
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<List<QueryRoleDto>> GetRoleList(string teamId)
        {
            var query = db.Queryable<Role, Team>((a, b) => new JoinQueryInfos(
                        JoinType.Inner, a.TeamId == b.ID
                ))
                .Where((a, b) => a.TeamId == teamId && a.Name != AppConsts.RoleName.Creator && a.IsDeleted == false)
                .Where((a, b) => b.IsDeleted == false)
                .Select(a => new QueryRoleDto
                {
                    RoleId = a.ID,
                    RoleName = a.Name
                }).OrderBy("a.CreateTime,a.TimeStamp asc");

            return await query.ToListAsync();
        }
    }
}
