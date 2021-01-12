using dy.IRepository;
using dy.IServices;
using dy.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dy.Services
{
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
    {
        private readonly IBaseRepository<TEntity> baseDal;


        public BaseServices(IBaseRepository<TEntity> baseRepository)
        {
            baseDal = baseRepository;
        }

        //public async Task<bool> Add(TEntity model)
        //{
        //    return await baseDal.Add(model);
        //}

        public Task<int> DeleteByIds(object[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> QueryByID(object objId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(TEntity model)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<TEntity>> ToPageList(int pageIndex, int pageSize)
        {
            return await baseDal.ToPageList(pageIndex, pageSize);
        }
    }
}
