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
    public class TeamMemberServices : BaseServices<TeamMember>, ITeamMemberServices
    {
        private readonly ITeamMemberRepository _memberRepository;

        public TeamMemberServices(IBaseRepository<TeamMember> baseRepository, ITeamMemberRepository memberRepository)
            : base(baseRepository)
        {
            _memberRepository = memberRepository;
        }

        /// <summary>
        /// 添加团队
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> PostTeamMemberAsync(AddTeamMemberDto dto, string openId)
        {
            return await _memberRepository.PostTeamMemberAsync(dto, openId);
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
            return await _memberRepository.GetTeamMemberListAsync(teamId, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据成员Id移除成员
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> DelTeamMemberByIdAsync(string Id)
        {
            return await _memberRepository.DelTeamMemberByIdAsync(Id);
        }

        /// <summary>
        /// 修改我在团队显示的昵称
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamNickNameAsync(UpdateTeamNickNameDto dto)
        {
            return await _memberRepository.UpdateTeamNickNameAsync(dto);
        }

        /// <summary>
        /// 根据成员Id查询当前成员的角色
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<string> GetTeamMemberRoleAsync(string teamId, string openId)
        {
            return await _memberRepository.GetTeamMemberRoleAsync(teamId, openId);
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
            return await _memberRepository.GetMemberQuotaAsync(teamId, memberId, pageIndex, pageSize);
        }

        /// <summary>
        /// 修改成员的额度
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMemberQuotaAsync(UpdateMemberQuotaDto dto)
        {
            return await _memberRepository.UpdateMemberQuotaAsync(dto);
        }

        /// <summary>
        /// 获取当前成员在团队的昵称
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<GetTeamNickNameDto> GetTeamNickNameAsync(string teamId, string openId)
        {
            return await _memberRepository.GetTeamNickNameAsync(teamId, openId);
        }
    }
}
