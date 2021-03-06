﻿using dy.Api.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dy.Api.Filter
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _env;
        private readonly ILoggerHelper _loggerHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="loggerHelper"></param>
        public GlobalExceptionsFilter(IHostEnvironment env, ILoggerHelper loggerHelper)
        {
            _env = env;
            _loggerHelper = loggerHelper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var json = new JsonErrorResponse();
            json.Message = context.Exception.Message;//错误信息
            if (_env.IsDevelopment())
            {
                json.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息
            }
            context.Result = new InternalServerErrorObjectResult(json);

            //采用log4net 进行错误日志记录
            _loggerHelper.Error(json.Message, "出现未知异常", context.Exception);
        }

        /// <summary>
        /// 
        /// </summary>
        public class InternalServerErrorObjectResult : ObjectResult
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public InternalServerErrorObjectResult(object value) : base(value)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        public class JsonErrorResponse
        {
            /// <summary>
            /// 生产环境的消息
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// 开发环境的消息
            /// </summary>
            public string DevelopmentMessage { get; set; }
        }
    }
}
