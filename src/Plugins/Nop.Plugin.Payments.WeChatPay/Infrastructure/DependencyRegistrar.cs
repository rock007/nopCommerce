using Autofac;
using Essensoft.AspNetCore.Payment.WeChatPay;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.Payments.WeChatPay.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //register client
            //builder.RegisterType<DefaultWeChatPayClient>().As<IWeChatPayClient>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultWeChatPayNotifyClient>().As<IWeChatPayNotifyClient>().InstancePerLifetimeScope();

            builder.RegisterType<ConfigureWeChatPayOptions>().As<IConfigureOptions<WeChatPayOptions>>().SingleInstance();
            builder.RegisterType<ConfigureHttpClientFactoryPayOptions>().As<IConfigureOptions<HttpClientFactoryOptions>>().SingleInstance();

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
