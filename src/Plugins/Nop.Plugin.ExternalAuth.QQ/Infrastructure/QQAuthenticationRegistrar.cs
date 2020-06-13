using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.QQ.Infrastructure
{
    /// <summary>
    /// Registration of Facebook authentication service (plugin)
    /// </summary>
    public class QQAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddQQ(QQDefaults.AuthenticationScheme, options =>
            {
                var settings = EngineContext.Current.Resolve<QQExternalAuthSettings>();

                options.AppId = settings.ClientKeyIdentifier;
                options.AppKey = settings.ClientSecret;
                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnRemoteFailure = ctx =>
                    {
                        ctx.HandleResponse();
                        ctx.Response.Redirect(ctx.Properties.GetString("ErrorCallback"));
                        return Task.FromResult(0);
                    }
                };
            });
        }
    }
}
