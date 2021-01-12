﻿using dy.Api.Log;
using dy.Common.Helper;
using dy.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dy.Api.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public CustomExceptionMiddleware(RequestDelegate next, ILoggerHelper logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex); // 日志记录
                await HandleExceptionAsync(httpContext, ex.Message);
            }
            finally
            {
                var statusCode = httpContext.Response.StatusCode;
                var msg = "";
                switch (statusCode)
                {
                    case 401:
                        msg = "未授权";
                        break;
                    case 403:
                        msg = "拒绝访问";
                        break;
                    case 404:
                        msg = "未找到服务";
                        break;
                    case 405:
                        msg = "405 Method Not Allowed";
                        break;
                    case 502:
                        msg = "请求错误";
                        break;
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    await HandleExceptionAsync(httpContext, msg);
                }
            }
        }
        ///
        private async Task HandleExceptionAsync(HttpContext httpContext, string msg)
        {
            ErrorModel error = new ErrorModel
            {
                code = httpContext.Response.StatusCode,
                msg = msg
            };
            var result = JsonHelper.toJson(error);
            httpContext.Response.ContentType = "application/json;charset=utf-8";
            await httpContext.Response.WriteAsync(result).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class CustomExceptionMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>();
        }
    }

}
