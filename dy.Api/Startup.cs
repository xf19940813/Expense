using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using dy.Api.Filter;
using dy.Api.JsonConv;
using dy.Api.Log;
using dy.Api.Middleware;
using dy.Api.SetUp;
using dy.Common.Helper;
using dy.Common.Redis;
using dy.Repository.sugar;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dy.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// log4net �ִ���
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //ע��appsettings��ȡ��
            services.AddSingleton(new AppSettings(Configuration));

            //ע��Redis
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            //logע��ILoggerHelper
            services.AddSingleton<ILoggerHelper, LogHelper>();

            //���ݿ�����
            BaseDBConfig.ConnectionString = Configuration.GetSection("AppSettings:ConnectionString").Value;

            //log4net
            repository = LogManager.CreateRepository("dy.Api");//��Ҫ��ȡ��־�Ĳֿ�����Ҳ������ĵ�Ȼ��Ŀ��
            XmlConfigurator.Configure(repository, new FileInfo("Log4net.config"));//ָ�������ļ���

            //ע��swagger
            services.AddSwaggerSetup();

            //jwt��Ȩ��֤
            services.AddAuthorizationSetup();

            //ע��automapper
            services.AddAutoMapper(typeof(Startup));

            //ȫ���쳣����
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionsFilter));
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
 
            }); ;

            //services.AddMemoryCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/V1/swagger.json", "dy.Api V1");

                //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�,ע��localhost:8001/swagger�Ƿ��ʲ����ģ�ȥlaunchSettings.json��launchUrlȥ����������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "doc";
                c.RoutePrefix = "";
            });

            app.UseCustomExceptionMiddleware();
            //ע���м����˳��UseRouting������ǰ�ߣ�UseAuthentication��UseAuthorizationǰ��
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseFileServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
