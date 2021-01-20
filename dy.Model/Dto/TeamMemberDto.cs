using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 成员列表信息
    /// </summary>
    public class QueryTeamMemberDto
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 当前团队里面显示的昵称
        /// </summary>
        public string TeamNickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime? JoinedTime { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 添加成员信息
    /// </summary>
    public class AddTeamMemberDto : AddUserInfoDto
    {
        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }
        
        /// <summary>
        /// 邀请人的Token，根据邀请人Token获取邀请人的UserId
        /// </summary>
        public string InviterToken { get; set; }

    }

    /// <summary>
    /// 更新我在团队显示的昵称
    /// </summary>
    public class UpdateTeamNickNameDto
    {
        /// <summary>
        /// 成员Id
        /// </summary>
        public string MemberId { get; set; }
        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// 当前团队里面显示的昵称
        /// </summary>
        public string TeamNickName { get; set; }
    }

    /// <summary>
    /// 成员额度
    /// </summary>
    public class QueryMemberQuotaDto
    {
        /// <summary>
        /// 成员Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 当前团队里面显示的昵称
        /// </summary>
        public string TeamNickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 免审额度
        /// </summary>
        public decimal FreeQuota { get; set; }
    }

    /// <summary>
    /// 修改成员额度
    /// </summary>
    public class UpdateMemberQuotaDto
    {
        /// <summary>
        /// 成员Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 免审额度
        /// </summary>
        public decimal FreeQuota { get; set; }
    }

    /// <summary>
    /// 获取成员昵称
    /// </summary>
    public class GetTeamNickNameDto
    {
        /// <summary>
        /// 成员Id
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// 在团队里的昵称
        /// </summary>
        public string TeamNickName { get; set; }
    }
}
