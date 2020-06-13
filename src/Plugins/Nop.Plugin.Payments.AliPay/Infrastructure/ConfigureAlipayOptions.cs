using Essensoft.AspNetCore.Payment.Alipay;
using Microsoft.Extensions.Options;
using Nop.Core.Infrastructure;
using Nop.Data;

namespace Nop.Plugin.Payments.AliPay.Infrastructure
{

    internal class ConfigureAlipayOptions : IConfigureOptions<AlipayOptions>
    {
        //private string[] _schemes;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfigureAlipayOptions()// string[] schemes, IHttpContextAccessor httpContextAccessor
        {
            //_schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            //_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Configure(AlipayOptions options)
        {
            //throw new NotImplementedException();
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var settings = EngineContext.Current.Resolve<AliPayPaymentSettings>();

                options.RsaPublicKey = settings.RsaPublicKey;
                options.AppId = settings.AppId;
                //options.Charset = settings.Charset;
                options.ServerUrl = settings.ServerUrl;
                //options.IsKeyFromFile = false;
                options.RsaPrivateKey = settings.RsaPrivateKey;
                options.SignType = settings.SignType;
                options.EncyptKey = settings.EncyptKey;
                //options.EncyptType
                options.Format = settings.Format;
                options.LogLevel = settings.LogLevel;
                options.Version = settings.Version;
            }
        }

        //public void PostConfigure(string name, AlipayOptions options)
        //{
        //    // no schemes means configure them all
        //    //if (_schemes.Length == 0 || _schemes.Contains(name))
        //    //{
        //    //    options.StateDataFormat = new DistributedCacheStateDataFormatter(_httpContextAccessor, name);
        //    //}
        //}
    }

    internal class PostConfigureAlipayOptions : IPostConfigureOptions<AlipayOptions>
    {
        //private string[] _schemes;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public PostConfigureAlipayOptions()// string[] schemes, IHttpContextAccessor httpContextAccessor
        {
            //_schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            //_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void PostConfigure(string name, AlipayOptions options)
        {
            // no schemes means configure them all
            //if (_schemes.Length == 0 || _schemes.Contains(name))
            //{
            //    options.StateDataFormat = new DistributedCacheStateDataFormatter(_httpContextAccessor, name);
            //}
        }
    }
}
