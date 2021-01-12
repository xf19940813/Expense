using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Common.Redis
{
    public interface IRedisCacheManager
    {
        #region 非异步
        //获取 Reids 缓存值
        string GetValue(string key);

        //获取值，并序列化
        TEntity Get<TEntity>(string key);

        //保存
        void Set(string key, object value, TimeSpan cacheTime);

        //判断是否存在
        bool Get(string key);

        //移除某一个缓存值
        void Remove(string key);

        //全部清除
        void Clear();
        #endregion

        #region 异步
        /// <summary>
        /// 获 Redis 缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValueAsync(string key);

        /// <summary>
        /// 获取值 并序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync<TEntity>(string key);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        Task SetAsync(string key, object value, TimeSpan cacheTime);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> GetAsync(string key);

        /// <summary>
        /// 移除某一个缓存值
        /// </summary>
        /// <param name="key"></param>
        Task RemoveAsync(string key);

        //根据关键字移除
        Task RemoveByKey(string key);

        /// <summary>
        /// 全部清除
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        void SetSysCustomKey(string customKey);
        #endregion
    }
}
