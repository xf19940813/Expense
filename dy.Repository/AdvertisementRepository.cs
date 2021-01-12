using dy.IRepository;
using dy.Model.Test;
using dy.Repository.sugar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace dy.Repository
{
    public class AdvertisementRepository : BaseRepository<Advertisement>, IAdvertisementRepository
    {
        //private DbContext context;
        //private SqlSugarClient db;
        //private SimpleClient<Advertisement> entityDB;

        //internal SqlSugarClient Db
        //{
        //    get { return db; }
        //    private set { db = value; }
        //}

        //public AdvertisementRepository()
        //{
        //    DbContext.Init(BaseDBConfig.ConnectionString);
        //    context = DbContext.GetDbContext();
        //    db = context.Db;
        //    entityDB = context.GetEntityDB<Advertisement>(db);
        //}

        //public DbContext Context
        //{
        //    get { return context; }
        //    set { context = value; }
        //}


        //public async Task<bool> Add(Advertisement model)
        //{
        //    //返回的i是long类型,这里你可以根据你的业务需要进行处理
        //    var i = await Task.Run(() => db.Insertable(model).ExecuteReturnBigIdentity());

        //    return i > 0;
        //}

        //public bool Delete(Advertisement model)
        //{
        //    throw new NotImplementedException();
        //}

        //public List<Advertisement> Query(Expression<Func<Advertisement, bool>> whereExpression)
        //{
        //    throw new NotImplementedException();
        //}

        //public int Sum(int i, int j)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Update(Advertisement model)
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<bool> PostAdvertisementAsync(Advertisement model)
        {
            //返回的i是long类型,这里你可以根据你的业务需要进行处理
            var i = await Task.Run(() => db.Insertable(model).ExecuteCommand());

            return i > 0;
        }
    }
}
