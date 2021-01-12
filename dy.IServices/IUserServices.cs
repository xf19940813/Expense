using dy.Model.Dto;
using dy.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IServices
{
    public interface IUserServices : IBaseServices<Wx_UserInfo>
    {
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> PostUserAsync(AddUserInfoDto input, string tokenHeader);

        /// <summary>
        /// 查询用户是否存在
        /// </summary>
        /// <returns></returns>
        Task<Wx_UserInfo> GetUserInfoExists(string tokenHeader);

        Task<bool> PostWx_UserOpenIdAsync(string code);
    }
}
