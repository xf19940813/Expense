using AutoMapper;
using dy.Model.Dto;
using dy.Model.Expense;
using dy.Model.Setting;
using dy.Model.Test;
using dy.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dy.Api.AutoMapper
{
    /// <summary>
    /// 实体模型关系映射
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public AutoMapperProfile()
        {
            //CreateMap<Advertisement, AdvertisementInput>();
            CreateMap<AddUserInfoDto, Wx_UserInfo>(); //用户信息

            CreateMap<AddExpenseInfoDto, ExpenseInfo>(); //报销信息

            CreateMap<AddTeamDto, Team>(); //团队

            CreateMap<AddTeamMemberDto, TeamMember>(); //团队成员

            CreateMap<AddRoleDto, Role>(); //角色

            CreateMap<AddTeamMemberDto, Wx_UserInfo>(); //用户信息
        }
    }
}
