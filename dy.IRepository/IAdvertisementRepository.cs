using dy.Model.Test;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace dy.IRepository
{
    public interface IAdvertisementRepository : IBaseRepository<Advertisement>
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> PostAdvertisementAsync(Advertisement model);
    }
}
