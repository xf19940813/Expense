using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Model.User
{
    /// <summary>
    /// 用户OpenId以及登录时间
    /// </summary>
    [SugarTable("tab_Wx_UserOpenId")]
    public class Wx_UserOpenId
    {
        ///<summary>
        /// 主键ID，如果是主键，此处必须指定，否则会引发InSingle(id)方法异常。
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
    }
}
