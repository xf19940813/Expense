using dy.Model;
using dy.Model.Dto;
using dy.Model.Expense;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IServices
{
    public interface ITeamServices : IBaseServices<Team>
    {
        /// <summary>
        /// 添加团队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> PostTeamAsync(AddTeamDto input, string tokenHeader);

        /// <summary>
        /// 团队列表分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页多少条</param>
        /// <param name="openId">用户唯一标识</param>
        /// <returns></returns>
        Task<PageResult<QueryTeamDto>> GetTeamListAsync(int pageIndex, int pageSize, string openId);


        /// <summary>
        /// 更新团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<bool> UpdateTeamAsync(UpdateTeamDto dto, string openId);

        /// <summary>
        /// 根据Id删除团队
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdAsync(string Id);

        /// <summary>
        /// 根据团队Id获取团队信息
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<GetTeamDto> GetTeamByIdAsync(string teamId);

        /// <summary>
        /// 团队转让
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<bool> TransferTeamAsync(TransferTeamDto dto, string openId);

        /// <summary>
        /// 团队启用/禁用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> IsEnabledAsync(IsEnabledDto dto);
    }
}
