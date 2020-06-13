//using Alipay.AopSdk.AspnetCore;
using Essensoft.AspNetCore.Payment.Alipay;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Payments.AliPay.Infrastructure
{

    public class PluginStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add Ali Pay Client
            services.AddHttpClient();

            // 引入Payment 依赖注入
            services.AddAlipay();

            // NopEngine 注入服务的流程为：
            // 1、INopStartup
            // 2、AddAutoMapper： IOrderedMapperProfile
            // 3、RegisterDependencies： IDependencyRegistrar
            // 4、RunStartupTasks： IStartupTask
            // 从这里可以看出，在INopStartup里是无法获取到 业务级别的服务的，
            // 只能获取到asp.net core框架本身的服务
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 12;
    }
}
