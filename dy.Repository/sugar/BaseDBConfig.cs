using System;
using System.Collections.Generic;
using System.Text;

namespace dy.Repository.sugar
{
    public class BaseDBConfig
    {
        //正常格式是

        //public static string ConnectionString = "server=127.0.0.1;uid=sa;pwd=123456;database=ApiLog";

        //原谅我用配置文件的形式，因为我直接调用的是我的服务器账号和密码，安全起见

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString { get; set; }
    }
}
