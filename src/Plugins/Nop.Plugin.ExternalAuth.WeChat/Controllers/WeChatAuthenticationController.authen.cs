using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.WeChat;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Web.Framework.Controllers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.WeChat.Controllers
{
    public partial class WeChatAuthenticationController : BasePluginController
    {
        #region Methods

        public IActionResult Login(string returnUrl)
        {
            /**
            if (!_externalAuthenticationService.ExternalAuthenticationMethodIsAvailable(WeChatAuthenticationDefaults.ProviderSystemName))
                throw new NopException("WeChat authentication module cannot be loaded");
            ***/
            if (string.IsNullOrEmpty(_wechatExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_wechatExternalAuthSettings.ClientSecret))
                throw new NopException("WeChat authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "WeChatAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString("ErrorCallback", Url.RouteUrl("Login", new { returnUrl }));

            return Challenge(authenticationProperties, WeChatDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate WeChat user
            var authenticateResult =  await this.HttpContext.AuthenticateAsync(WeChatDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = WeChatAuthenticationDefaults.ProviderSystemName,
                AccessToken = await this.HttpContext.GetTokenAsync(WeChatDefaults.AuthenticationScheme, "access_token"),
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }

        #endregion
    }
}