using AutoMapper;
using dy.Common.Config;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.IRepository;
using dy.Model.Dto;
using dy.Model.Expense;
using dy.Model.User;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dy.Repository
{
    public class UserRepository : BaseRepository<Wx_UserInfo>, IUserRepository
    {
        private readonly IMapper iMapper;
        private readonly IRedisCacheManager _redisCacheManager;
        public UserRepository(IMapper IMapper, IRedisCacheManager redisCacheManager)
        {
            iMapper = IMapper;
            _redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> PostUserAsync(AddUserInfoDto input, string openId, string sessionKey)
        {
            Wx_UserInfo user = iMapper.Map<Wx_UserInfo>(input);
            user.ID = IdHelper.CreateGuid();

            if (openId != null)
            {
                user.OpenId = openId;
                if (input.EncryptedData != "" && input.IV != "")
                {
                    user.MobilePhone = WxHelper.getPhoneNumber(input.EncryptedData, input.IV, sessionKey);
                }
            }

            var result = 0;
            return await Task.Run(() =>
            {
                var isAny = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).Any();
                
                if (isAny == false)
                {
                    result = db.Insertable(user).ExecuteCommand();
                }
                else
                {
                    if (user.MobilePhone == null)
                        throw new Exception("没有获取到用户手机号码！");

                    result = db.Updateable<Wx_UserInfo>().SetColumns(a => new Wx_UserInfo() { MobilePhone = user.MobilePhone, FollowDate = DateTime.Now })
                                .Where(a => a.OpenId == openId).ExecuteCommand();

                    if(result > 0)
                    {
                        if(input.TeamId != null)
                        {
                            var userId = db.Queryable<Wx_UserInfo>().Where(a => a.OpenId == openId).First()?.ID;

                            db.Updateable<TeamMember>().SetColumns(a => new TeamMember()
                            {
                                MobilePhone = user.MobilePhone
                            }).Where(a => a.TeamId == input.TeamId && a.JoinedUserId == userId && a.IsDeleted == false).ExecuteCommand();
                        }
                    }
                }
                return result > 0;
            });

        }

        /// <summary>
        /// 查询用户是否存在
        /// </summary>
        /// <returns></returns>

        public async Task<Wx_UserInfo> GetUserInfoExists(string tokenHeader)
        {
            string OpenId = string.Empty;
            bool isKey = _redisCacheManager.Get(tokenHeader);
            
            if(isKey)
            {
                OpenId = _redisCacheManager.GetValue(tokenHeader).ToString().Split(";")[0].Trim('"');
            }

            return await Task.Run(() =>
            {
                var result = db.Queryable<Wx_UserInfo>().Where(it => it.OpenId == OpenId).FirstAsync();

                return result;
            });
        }

        /// <summary>
        /// 添加用户登录信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> PostWx_UserOpenIdAsync(string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + WxConfig.AppId + "&secret=" + WxConfig.AppSecret + "&js_code=" + code + "&grant_type=authorization_code";

            Wx_UserOpenId userLog = new Wx_UserOpenId();
            userLog.ID = IdHelper.CreateGuid();
            string returnText = WxHelper.GetResponse(url);

            if (returnText != "")
            {
                JObject obj = Newtonsoft.Json.Linq.JObject.Parse(returnText);
                string openid = obj["openid"].ToString();
                string sessionkey = obj["session_key"].ToString();
                userLog.OpenId = openid;

                _redisCacheManager.Remove("myOpenId");
                _redisCacheManager.Remove("mySessionKey");
                _redisCacheManager.Set("myOpenId", openid, TimeSpan.FromHours(2));
                _redisCacheManager.Set("mySessionKey", sessionkey, TimeSpan.FromHours(2));
            }
            userLog.LoginTime = DateTime.Now;

            var result = 0;
            return await Task.Run(() =>
            {
                var isAny = db.Queryable<Wx_UserOpenId>().Where(it => it.OpenId == userLog.OpenId).Any();//是否存在
                if (isAny == false)
                {
                    result = db.Insertable(userLog).ExecuteCommand();
                }
                else 
                {
                    result = db.Updateable<Wx_UserOpenId>().SetColumns(it => new Wx_UserOpenId() { LoginTime = userLog.LoginTime })
                                .Where(it => it.OpenId == userLog.OpenId).ExecuteCommand();
                }

                return result > 0;
            });
        }
    }
}
