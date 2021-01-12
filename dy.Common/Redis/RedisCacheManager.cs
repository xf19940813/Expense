using dy.Common.Helper;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Common.Redis
{
    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly string redisConnenctionString;
        public volatile ConnectionMultiplexer redisConnection;
        private readonly object redisConnectionLock = new object();

        public string CustomKey { get; set; }

        public RedisCacheManager()
        {
            string redisConfiguration = AppSettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" }); //获取连接字符串

            if (string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config is empty", nameof(redisConfiguration));
            }

            this.redisConnenctionString = redisConfiguration;
            this.redisConnection = GetRedisConnection();
        }

        /// <summary>
        /// 核心代码 获取连接实例
        /// 通过if 加 lock的方式，实现单例模式
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this.redisConnection != null && this.redisConnection.IsConnected)
            {
                return this.redisConnection;
            }

            //加锁，防止异步编程中出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (this.redisConnection != null)
                {
                    //释放redis连接
                    this.redisConnection.Dispose();
                }

                try
                {
                    this.redisConnection = ConnectionMultiplexer.Connect(redisConnenctionString);
                }
                catch (Exception)
                {
                    throw new Exception("Redis服务未开启，请开启该服务");
                }
            }

            return this.redisConnection;
        }


        #region 非异步
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }

        public bool Get(string key)
        {
            key = AddSysCustomKey(key);
            return redisConnection.GetDatabase().KeyExists(key);
        }

        public string GetValue(string key)
        {
            key = AddSysCustomKey(key);
            return redisConnection.GetDatabase().StringGet(key);
        }

        public TEntity Get<TEntity>(string key)
        {
            key = AddSysCustomKey(key);
            var value = redisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }

        public void Remove(string key)
        {
            key = AddSysCustomKey(key);
            redisConnection.GetDatabase().KeyDelete(key);
        }

        public void Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                key = AddSysCustomKey(key);
                //序列化，将object值生成RedisValue
                redisConnection.GetDatabase().StringSet(key, SerializeHelper.Serialize(value), cacheTime);
            }
        }

        public bool SetValue(string key, byte[] value)
        {
            key = AddSysCustomKey(key);
            return redisConnection.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(120));
        }
        #endregion


        #region 异步
        public async Task ClearAsync()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    await redisConnection.GetDatabase().KeyDeleteAsync(key);
                }
            }
        }

        public async Task<TEntity> GetAsync<TEntity>(string key)
        {
            key = AddSysCustomKey(key);
            var value = await redisConnection.GetDatabase().StringGetAsync(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default;
            }

        }

        public async Task<bool> GetAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await redisConnection.GetDatabase().KeyExistsAsync(key);
        }

        public async Task<string> GetValueAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await redisConnection.GetDatabase().StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            key = AddSysCustomKey(key);
            await redisConnection.GetDatabase().KeyDeleteAsync(key);
        }

        public async Task SetAsync(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                key = AddSysCustomKey(key);
                //序列化 将 object 值生成Redisvalue
                await redisConnection.GetDatabase().StringSetAsync(key, SerializeHelper.Serialize(value), cacheTime);
            }
        }

        public async Task<bool> setValue(string key, byte[] value)
        {
            key = AddSysCustomKey(key);
            return await redisConnection.GetDatabase().StringSetAsync(key, value, TimeSpan.FromSeconds(120));
        }


        public async Task RemoveByKey(string key)
        {
            var redisResult = await redisConnection.GetDatabase().ScriptEvaluateAsync(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = key });

            if (!redisResult.IsNull)
            {
                var keys = (string[])redisResult;
                foreach (var k in keys)
                    redisConnection.GetDatabase().KeyDelete(k);

            }
        }

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetSysCustomKey(string customKey)
        {
            CustomKey = customKey;
        }

        private string AddSysCustomKey(string oldKey)
        {
            var prefixKey = CustomKey ?? RedisConfig.SysCustomKey;
            return prefixKey + oldKey;
        }
        #endregion
    }
}
