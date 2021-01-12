using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dy.Api.Log;
using dy.Common.Redis;
using dy.IServices;
using dy.Model.Test;
using dy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dy.Api.Controllers
{
    /// <summary>
    /// 测试Api
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestController : BaseController
    {
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly IAdvertisementServices _advertisementService;
        private readonly ILoggerHelper _logger;
        //IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="advertisementService"></param>
        /// <param name="loggerHelper"></param>
        /// <param name="redisCacheManager"></param>
        public TestController(IAdvertisementServices advertisementService, ILoggerHelper loggerHelper, IRedisCacheManager redisCacheManager)
        {
            _advertisementService = advertisementService;
            _logger = loggerHelper;
            _redisCacheManager = redisCacheManager;
        }
        
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetAdvertisementAsync")]
        public async Task<IActionResult> GetAdvertisementAsync(int pageIndex, int pageSize)
        {
            var data = await _advertisementService.ToPageList(pageIndex, pageSize);
            return Ok(data);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("PostAdvertisementAsync")]
        public async Task<IActionResult> PostAdvertisementAsync(AdvertisementInput input)
        {

            try
            {
                await _advertisementService.PostAdvertisementAsync(input);
                return AddSuccessMsg();
            }
            catch (Exception err)
            {
                _logger.Error("添加测试数据失败", err.Message);
                return FailedMsg("添加测试数据失败！" + err.Message);   
            }
        }

        /// <summary>
        /// 测试日志
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("LogTest")]
        public IActionResult LogTest()
        {
            try
            {
                _logger.Error(typeof(TestController), "这是错误日志", new Exception("123"));
                _logger.Debug(typeof(TestController), "这是bug日志");
                return SuccessMsg();
            }
            catch(Exception err)
            {
                _logger.Error("log日志记录失败!", err.Message);
                return FailedMsg("log日志记录失败！" + err.Message);
            }
        }

    }
}
