using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Expense
{
    /// <summary>
    /// 团队成员实体
    /// </summary>
    [SugarTable("tab_TeamMember")]
    public class TeamMember
    {
        /// <summary>
        ///  主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 当前团队里面显示的昵称
        /// </summary>
        public string TeamNickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 团队Id
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 邀请人
        /// </summary>
        public string InviterUserId { get; set; }
        /// <summary>
        /// 加入人
        /// </summary>
        public string JoinedUserId { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastModifyUserId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 免审额度
        /// </summary>
        public decimal FreeQuota { get; set; }
    }
}
