using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Notification.Hubs;
using Nop.Plugin.Widgets.Notification.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Widgets.Notification.Infrastructure
{
    public class PluginSignalRStartup: INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, UsernameUserIdProvider>();

        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            // SignalR
            application.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/notificationHub");
                //routes.MapHub<ProgressHub>("/progressDemo");
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order =>901;
    }
}
