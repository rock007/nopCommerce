using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Essensoft.AspNetCore.Payment.WeChatPay;
using Nop.Data;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.Payments.WeChatPay.Infrastructure
{
    internal class ConfigureWeChatPayOptions : IConfigureOptions<WeChatPayOptions>
    {
        public ConfigureWeChatPayOptions()
        {

        }

        public void Configure(WeChatPayOptions options)
        {
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var settings = EngineContext.Current.Resolve<WeChatPayPaymentSettings>();

                if(!String.IsNullOrEmpty( settings.RsaPublicKey))
                {
                    options.RsaPublicKey = settings.Key;
                }
                options.AppId = settings.AppId;
                options.Secret = settings.AppSecret;
                options.MchId = settings.MerchantId;
                options.Key = settings.Key;
                options.LogLevel = settings.LogLevel;
            }
        }
    }

    //internal class PostConfigureWeChatPayOptions : IPostConfigureOptions<WeChatPayOptions>
    //{
    //    public PostConfigureWeChatPayOptions()
    //    {

    //    }
    //    public void PostConfigure(string name, WeChatPayOptions options)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    // 

    internal class ConfigureHttpClientFactoryPayOptions : IConfigureOptions<HttpClientFactoryOptions>
    {
        public ConfigureHttpClientFactoryPayOptions()
        {

        }

        public void Configure(HttpClientFactoryOptions options)
        {
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var settings = EngineContext.Current.Resolve<WeChatPayPaymentSettings>();
                options.HttpMessageHandlerBuilderActions.Add(b =>
                {
                    b.Name = "wechatpayCertificateName";
                    // HttpClientHandler : HttpMessageHandler
                    var certificate = new X509Certificate2(settings.SslCertificationPath, 
                        settings.SslCertificationPassword, 
                        X509KeyStorageFlags.MachineKeySet);
                    var handler = new HttpClientHandler();
                    handler.ClientCertificates.Clear();
                    handler.ClientCertificates.Add(certificate);

                    b.PrimaryHandler = handler;
                });
            }
        }
    }
}
