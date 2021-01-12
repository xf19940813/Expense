using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 添加团队
    /// </summary>
    public class AddTeamDto
    {
        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 团队信息
        /// </summary>
        public string TeamInfo { get; set; }
    }


    /// <summary>
    /// 更新团队信息
    /// </summary>
    public class UpdateTeamDto : AddTeamDto
    {
        /// <summary>
        /// 团队Id
        /// </summary>
        public string ID { get; set; }
    }

    /// <summary>
    /// 查询团队信息
    /// </summary>
    public class QueryTeamDto
    {
        /// <summary>
        /// 团队ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

    }

    /// <summary>
    /// 获取团队信息
    /// </summary>
    public class GetTeamDto : QueryTeamDto
    {
        /// <summary>
        ///团队信息
        /// </summary>
        public string TeamInfo { get; set; }
    }

    /// <summary>
    /// 团队转让
    /// </summary>
    public class TransferTeamDto
    {
        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// 成员Id
        /// </summary>
        public string MemberId { get; set; }
    }

    /// <summary>
    /// 团队启用/禁用
    /// </summary>
    public class IsEnabledDto
    {
        /// <summary>
        /// 团队Id
        /// </summary>
       public string TeamId { get; set; }

       /// <summary>
       /// switch的值 true 和 false
       /// </summary>
       public string Value { get; set; }
    }
}
