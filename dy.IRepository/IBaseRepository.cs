using dy.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace dy.IRepository
{
    /// <summary>
    /// 基类接口,其他接口继承该接口
    /// </summary>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 条件查询列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 条件查询一条返回实体
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<TEntity> Query(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PageResult<TEntity>> ToPageList(int pageIndex, int pageSize);


        /// <summary>
        /// 根据ID查询
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        Task<TEntity> GetById(object objId);

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetList();

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity model);

        /// <summary>
        /// 根据主键数组删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> DeleteByIds(object[] ids);
    }
}
