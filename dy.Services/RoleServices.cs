using dy.IRepository;
using dy.IServices;
using dy.Model.Dto;
using dy.Model.Setting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Services
{
    public class RoleServices : BaseServices<Role>, IRoleServices
    {
        private readonly IRoleRepository _roleDal;

        public RoleServices(IBaseRepository<Role> baseRepository, IRoleRepository roleDal)
            : base(baseRepository)
        {
            _roleDal = roleDal;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> PostRoleAsync(AddRoleDto dto, string openId)
        {
            return await _roleDal.PostRoleAsync(dto, openId);
        }

        /// <summary>
        /// 修改成员角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<QueryRoleDto> UpdateTeamMemberRoleAsync(UpdateRoleDto dto, string openId)
        {
            return await _roleDal.UpdateTeamMemberRoleAsync(dto, openId);
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<List<QueryRoleDto>> GetRoleList(string teamId)
        {
            return await _roleDal.GetRoleList(teamId);
        }
    }
}
