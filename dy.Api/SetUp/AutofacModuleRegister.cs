﻿using Autofac;
using Autofac.Extras.DynamicProxy;
using dy.Api.AOP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dy.Api.SetUp
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //注册RedisAop
            builder.RegisterType<RedisCacheAOP>();
            //注册Service
            var assemblysServices = Assembly.Load("dy.Services");
            builder.RegisterAssemblyTypes(assemblysServices)
                .InstancePerDependency()//瞬时单例
               .AsImplementedInterfaces()////自动以其实现的所有接口类型暴露（包括IDisposable接口）
               .EnableInterfaceInterceptors() //引用Autofac.Extras.DynamicProxy;
               .InterceptedBy(typeof(RedisCacheAOP));//可以放一个AOP拦截器集合

            //注册Repository
            var assemblysRepository = Assembly.Load("dy.Repository");
            builder.RegisterAssemblyTypes(assemblysRepository)
                .InstancePerDependency()//瞬时单例
               .AsImplementedInterfaces()////自动以其实现的所有接口类型暴露（包括IDisposable接口）
               .EnableInterfaceInterceptors(); //引用Autofac.Extras.DynamicProxy;
        }
    }
}
