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
    public class TeamServices : BaseServices<Team>, ITeamServices
    {
        private readonly ITeamRepository _teameDal;

        public TeamServices(IBaseRepository<Team> baseRepository, ITeamRepository teameDal)
             : base(baseRepository)
        {
            _teameDal = teameDal;
        }

        /// <summary>
        /// 添加团队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> PostTeamAsync(AddTeamDto input, string tokenHeader)
        {
            return await _teameDal.PostTeamAsync(input, tokenHeader);
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
            return await _teameDal.GetTeamListAsync(pageIndex, pageSize, openId);
        }

        /// <summary>
        /// 更新团队信息
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamAsync(UpdateTeamDto dto, string openId)
        {
            return await _teameDal.UpdateTeamAsync(dto, openId);
        }

        /// <summary>
        /// 根据Id删除团队
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdAsync(string Id)
        {
            return await _teameDal.DeleteByIdAsync(Id);
        }

        /// <summary>
        /// 根据团队Id获取团队信息
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<GetTeamDto> GetTeamByIdAsync(string teamId)
        {
            return await _teameDal.GetTeamByIdAsync(teamId);
        }

        /// <summary>
        /// 团队转让
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> TransferTeamAsync(TransferTeamDto dto, string openId)
        {
            return await _teameDal.TransferTeamAsync(dto, openId);
        }

        /// <summary>
        /// 团队启用/禁用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> IsEnabledAsync(IsEnabledDto dto)
        {
            return await _teameDal.IsEnabledAsync(dto);
        }
    }
}
