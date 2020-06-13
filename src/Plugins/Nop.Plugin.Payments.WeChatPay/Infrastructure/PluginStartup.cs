using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Essensoft.AspNetCore.Payment.WeChatPay;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Data;
using Nop.Core.Infrastructure;


namespace Nop.Plugin.Payments.WeChatPay.Infrastructure
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
            //add WeChat Pay Client
            services.AddHttpClient();
            services.AddHttpClient("wechatpayCertificateName")
                .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (DataSettingsManager.DatabaseIsInstalled)
                {
                    var settings = EngineContext.Current.Resolve<WeChatPayPaymentSettings>();
                    var certificate = new X509Certificate2(
                    settings.SslCertificationPath, // 证书存放路径
                    settings.SslCertificationPassword, // 证书的密码
                    X509KeyStorageFlags.MachineKeySet);
                    handler.ClientCertificates.Add(certificate);
                }
                return handler;
            });

            services.AddWeChatPay();
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
