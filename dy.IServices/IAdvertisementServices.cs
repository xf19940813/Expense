using dy.Model.Test;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace dy.IServices
{
    public interface IAdvertisementServices : IBaseServices<Advertisement>
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> PostAdvertisementAsync(AdvertisementInput input);
    }
}
