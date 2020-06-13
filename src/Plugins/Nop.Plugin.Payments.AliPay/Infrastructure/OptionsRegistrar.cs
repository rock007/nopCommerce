using System;
using System.Collections.Generic;
using System.Text;
using Essensoft.AspNetCore.Payment.Alipay;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.Payments.AliPay.Infrastructure
{
    public class OptionsRegistrar: IOptionsRegistrar
    {

        public int Order
        {
            get { return 2; }
        }

        public void Register(IServiceCollection services)
        {
            //if (DataSettingsManager.DatabaseIsInstalled)
            //{
            //    var settings = EngineContext.Current.Resolve<AliPayPaymentSettings>();
            //    //services.AddAlipay(options => {
            //    //    options.AlipayPublicKey = settings.RsaPublicKey;
            //    //    options.AppId = settings.AppId;
            //    //    options.CharSet = settings.Charset;
            //    //    options.Gatewayurl = settings.ServerUrl;
            //    //    options.IsKeyFromFile = false;
            //    //    options.PrivateKey = settings.RsaPrivateKey;
            //    //    options.SignType = settings.SignType;
            //    //    options.Uid = "";
            //    //});
            //    //services.Configure
            //    services.PostConfigure<AlipayOptions>(options =>
            //    {
            //        options.RsaPrivateKey = settings.RsaPublicKey;
            //        options.AppId = settings.AppId;
            //        //options.Charset = settings.Charset;
            //        options.ServerUrl = settings.ServerUrl;
            //        //options.IsKeyFromFile = false;
            //        options.RsaPrivateKey = settings.RsaPrivateKey;
            //        options.SignType = settings.SignType;
            //        options.EncyptKey = settings.EncyptKey;
            //        //options.EncyptType
            //        options.Format = settings.Format;
            //        options.LogLevel = settings.LogLevel;
            //        options.Version = settings.Version;
            //    });
            //}
        }
    }
}
