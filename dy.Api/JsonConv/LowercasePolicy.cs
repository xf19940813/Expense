﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace dy.Api.JsonConv
{
    /// <summary>
    /// 返回对象全小写
    /// </summary>
    public class LowercasePolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
             name.ToLower();
    }
}
