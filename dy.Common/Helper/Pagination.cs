using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Common.Helper
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public class Pagination
    {
        public int Page { get; set; } = 1;
        public int limit { get; set; } = 20;

        public int PageSize
        {
            get { return limit; }
            set
            {
                this.limit = value;
            }
        }

        // int _page = 1;

        // int 
        //// int _pageSize = 20;
        // public Pagination()
        // {
        // }
        // public Pagination(int page, int pageSize)
        // {
        //     this.Page = page;
        //     this.PageSize = pageSize;
        // }
        // /// <summary>
        // /// 当前页
        // /// </summary>
        // public int Page { get { return this._page; } set { this._page = value; } }

        // /// <summary>
        // /// 页大小
        // /// </summary>
        // public int PageSize { get { return this._pageSize; } set { this._pageSize = value; } }

        public PagedData ToPagedData()
        {
            PagedData pageData = new PagedData(this);
            return pageData;
        }
        public PagedData<T> ToPagedData<T>()
        {
            PagedData<T> pageData = new PagedData<T>(this);
            return pageData;
        }
    }
}
