using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.WeChat;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.WeChat.Infrastructure
{
    /// <summary>
    /// Registration of Facebook authentication service (plugin)
    /// </summary>
    public class WeChatAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddWeChat(WeChatDefaults.AuthenticationScheme, options =>
            {
                var settings = EngineContext.Current.Resolve<WeChatExternalAuthSettings>();

                options.AppId = settings.ClientKeyIdentifier;
                options.AppSecret = settings.ClientSecret;
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
