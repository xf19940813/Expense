using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace dy.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConsts
    {
        /// <summary>
        /// 审核状态
        /// </summary>
        public class AuditStatus
        {
            /// <summary>
            /// 未审核
            /// </summary>
            [Description("未审核")]
            public const int UnAudited = 0;

            /// <summary>
            /// 已审核
            /// </summary>
            [Description("已审核")]
            public const int Audited = 1;

            /// <summary>
            /// 已完结
            /// </summary>
            [Description("已完结")]
            public const int Finished = 2;
        }

        /// <summary>
        /// 消费类型
        /// </summary>
        public class ExpenseType
        {
            /// <summary>
            /// 日常办公
            /// </summary>
            [Description("日常办公")]
            public const int DailyWork = 0;

            /// <summary>
            /// 办公设备
            /// </summary>
            [Description("办公设备")]
            public const int OfficeEquipment = 1;

            /// <summary>
            /// 餐饮补助
            /// </summary>
            [Description("餐饮补助")]
            public const int FoodSubsidies = 2;

            /// <summary>
            /// 其他
            /// </summary>
            [Description("其他")]
            public const int Other = 3;
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        public class RoleName
        {
            /// <summary>
            /// 创建者
            /// </summary>
            public const string Creator = "创建者";

            /// <summary>
            /// 管理员
            /// </summary>
            public const string Admin = "管理员";

            /// <summary>
            /// 财务
            /// </summary>
            public const string Finance = "财务";

            /// <summary>
            /// 普通员工
            /// </summary>
            public const string Ordinary = "普通员工";

            /// <summary>
            /// 团队黑名单
            /// </summary>
            public const string BlackList = "黑名单";
        }

        /// <summary>
        /// 付款方式
        /// </summary>
        public class PaymentType
        {
            /// <summary>
            /// 立即支付
            /// </summary>
            [Description("立即支付")]
            public const int Promptly = 1;

            /// <summary>
            /// 财务确认
            /// </summary>
            [Description("财务确认")]
            public const int FinanceVerify = 2;

            /// <summary>
            /// 线下
            /// </summary>
            [Description("线下")]
            public const int Offline = 3;
        }
    }
}
