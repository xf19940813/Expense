using dy.Model.Dto;
using dy.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.IRepository
{
    public interface IUserRepository : IBaseRepository<Wx_UserInfo>
    {
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="input"></param>
        ///  <param name="tokenHeader"></param>
        /// <returns></returns>
        Task<bool> PostUserAsync(AddUserInfoDto input, string openId, string sessionKey);

        /// <summary>
        /// 查询用户是否存在
        /// </summary>
        /// <returns></returns>
        Task<Wx_UserInfo> GetUserInfoExists(string tokenHeader);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>

        Task<bool> PostWx_UserOpenIdAsync(string code);
    }
}
