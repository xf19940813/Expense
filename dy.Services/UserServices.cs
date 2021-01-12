using AutoMapper;
using dy.Common.Config;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IRepository;
using dy.IServices;
using dy.Model;
using dy.Model.Dto;
using dy.Model.User;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dy.Services
{
    public class UserServices : BaseServices<Wx_UserInfo>, IUserServices
    {
        private readonly IUserRepository _userDal;
        //private readonly IMapper iMapper;
        //private readonly IRedisCacheManager _redisCacheManager;

        public UserServices(IBaseRepository<Wx_UserInfo> baseRepository, IUserRepository userDal) 
            : base(baseRepository)
        {
            _userDal = userDal;
            //iMapper = IMapper;
            //_redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tokenHeader"></param>
        /// <returns></returns>
        public async Task<bool> PostUserAsync(AddUserInfoDto input, string tokenHeader)
        {
            return await _userDal.PostUserAsync(input, tokenHeader);
        }

        /// <summary>
        /// 根据openId查询用户是否存在
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<Wx_UserInfo> GetUserInfoExists(string tokenHeader)
        {
            return await _userDal.GetUserInfoExists(tokenHeader);
        }

        /// <summary>
        /// 添加登录日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> PostWx_UserOpenIdAsync(string code)
        {
            
            return await _userDal.PostWx_UserOpenIdAsync(code);
        }
    }
}
