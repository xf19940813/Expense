using dy.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.Dto
{
    /// <summary>
    /// 添加用户信息
    /// </summary>
    public class AddUserInfoDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

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
        /// 
        /// </summary>
        public string IV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EncryptedData { get; set; }
    }

    /// <summary>
    /// 查询用户信息
    /// </summary>
    public class QueryUserInfoDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }
    }
}
