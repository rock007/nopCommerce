using Autofac;
using Essensoft.AspNetCore.Payment.Alipay;
using Microsoft.Extensions.Options;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.Payments.AliPay.Infrastructure
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
            //builder.RegisterType<DefaultAliPayClient>().As<IAlipayClient>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultAliPayNotifyClient>().As<IAlipayNotifyClient>().InstancePerLifetimeScope();

            builder.RegisterType<ConfigureAlipayOptions>().As<IConfigureOptions<AlipayOptions>>().SingleInstance();
            builder.RegisterType<PostConfigureAlipayOptions>().As<IPostConfigureOptions<AlipayOptions>>().SingleInstance();
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
