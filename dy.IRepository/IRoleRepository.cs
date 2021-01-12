using dy.Model.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IRepository
{
    public interface IRoleRepository
    {
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<bool> PostRoleAsync(AddRoleDto dto, string openId);

        /// <summary>
        /// 修改成员角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<QueryRoleDto> UpdateTeamMemberRoleAsync(UpdateRoleDto dto);

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<List<QueryRoleDto>> GetRoleList(string teamId);

    }
}
