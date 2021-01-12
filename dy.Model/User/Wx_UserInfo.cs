using dy.Model.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.User
{
    /// <summary>
    /// 微信用户信息
    /// </summary>
    [SugarTable("tab_Wx_UserInfo")]
    public class Wx_UserInfo
    {
        /// <summary>
        /// 主键ID，如果是主键，此处必须指定，否则会引发InSingle(id)方法异常。
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime? FollowDate { get; set; }
        /// <summary>
        /// 性别，1代表男，2代表女
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
