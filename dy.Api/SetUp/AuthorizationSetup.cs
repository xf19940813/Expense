using dy.Common.Helper;
using dy.Common.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dy.Api.SetUp
{
    /// <summary>
    /// 注册服务
    /// </summary>
    public static class AuthorizationSetup
    {
        static IRedisCacheManager _redisCacheManager = new RedisCacheManager();

        public static void AddAuthorizationSetup(this IServiceCollection services)
        {
            //if (services == null) throw new ArgumentNullException(nameof(services));

            //读取配置文件
            var symmetricKeyAsBase64 = AppSettings.app(new string[] { "AppSettings", "JwtSetting", "SecretKey" });
            //var symmetricKeyAsBase64 = _redisCacheManager.GetValue("mySessionKey").ToString().Trim('"');
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = AppSettings.app(new string[] { "AppSettings", "JwtSetting", "Issuer" });
            var Audience = AppSettings.app(new string[] { "AppSettings", "JwtSetting", "Audience" });

            //令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, //是否验证SecurityKey
                IssuerSigningKey = signingKey,  // //拿到SecurityKey
                ValidateIssuer = true, //验证秘钥发行人，如果要验证在这里指定发行人字符串即可
                ValidIssuer = Issuer,//发行人
                ValidateAudience = true,  // 验证秘钥的订阅人(接受人)，如果要验证在这里提供接收人字符串即可
                ValidAudience = Audience, //订阅人
                ValidateLifetime = true, //是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew 
                ClockSkew = TimeSpan.FromSeconds(30), // 30秒有效
                RequireExpirationTime = true,  //token是否包含有效期 
            };

            //2.1【认证】、core自带官方JWT认证
            //开启Bearer认证
            services.AddAuthentication("Bearer")
                // 添加JwtBearer服务
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = tokenValidationParameters;
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // 如果过期，则把<是否过期>添加到，返回头信息中
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");

                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
