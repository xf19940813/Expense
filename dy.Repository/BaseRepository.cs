using dy.IRepository;
using dy.Model;
using dy.Repository.sugar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace dy.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        private DbContext context;
        public SqlSugarClient db;
        public SimpleClient<TEntity> entityDB;

        internal SqlSugarClient Db
        {
            get { return db; }
            private set { db = value; }
        }

        public BaseRepository()
        {
            DbContext.Init(BaseDBConfig.ConnectionString);
            context = DbContext.GetDbContext();
            db = context.Db;
            entityDB = context.GetEntityDB<TEntity>(db);
        }

        public DbContext Context
        {
            get { return context; }
            set { context = value; }
        }

        public async Task<bool> Add(TEntity model)
        {
            //返回的i是long类型,这里你可以根据你的业务需要进行处理
            var i = await Task.Run(() => db.Insertable(model).ExecuteReturnBigIdentity());

            return i > 0;
        }


        public Task<int> DeleteByIds(object[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetById(object objId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<TEntity>> ToPageList(int pageIndex, int pageSize)
        {
            return await Task.Run(() =>
            {
                PageResult<TEntity> pageResult = new PageResult<TEntity>();

                var data = entityDB.AsQueryable().ToPageList(pageIndex, pageSize);
                pageResult.totalCount = entityDB.AsQueryable().Count();
                pageResult.pageIndex = pageIndex;
                pageResult.pageSize = pageSize;
                pageResult.data = data;

                return pageResult;
            });
        }

        public Task<TEntity> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(TEntity model)
        {
            throw new NotImplementedException();
        }

    }
}
