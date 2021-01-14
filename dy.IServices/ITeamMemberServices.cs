using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IServices
{
    public interface ITeamMemberServices : IBaseServices<TeamMember>
    {
        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> PostTeamMemberAsync(AddTeamMemberDto dto, string openId, string sessionKey);

        /// <summary>
        /// 分页获取成员数据
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <returns></returns>
        Task<PageResult<QueryTeamMemberDto>> GetTeamMemberListAsync(string teamId, int pageIndex, int pageSize);

        /// <summary>
        /// 根据成员Id移除成员
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> DelTeamMemberByIdAsync(string Id);

        /// <summary>
        /// 修改我在团队显示的昵称
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> UpdateTeamNickNameAsync(UpdateTeamNickNameDto dto);

        /// <summary>
        /// 根据成员Id查询当前成员的角色
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<string> GetTeamMemberRoleAsync(string teamId, string openId);

        /// <summary>
        /// 成员额度分页查询
        /// </summary>
        /// <param name="teamId">团队Id</param>
        /// <param name="memberId">成员Id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResult<QueryMemberQuotaDto>> GetMemberQuotaAsync(string teamId, string memberId, int pageIndex, int pageSize);

        /// <summary>
        /// 修改成员额度
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> UpdateMemberQuotaAsync(UpdateMemberQuotaDto dto);

        /// <summary>
        /// 获取当前成员在团队的昵称
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<GetTeamNickNameDto> GetTeamNickNameAsync(string teamId, string openId);
    }
}
