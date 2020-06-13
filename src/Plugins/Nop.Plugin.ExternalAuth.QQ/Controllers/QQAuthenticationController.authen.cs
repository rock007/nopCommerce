using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.QQ.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.QQ.Controllers
{
    public partial class QQAuthenticationController : BasePluginController
    {
        #region Methods

        public IActionResult Login(string returnUrl)
        {

            /***!!
            if (!_externalAuthenticationService.ExternalAuthenticationMethodIsAvailable(QQAuthenticationDefaults.ProviderSystemName))
                throw new NopException("QQ authentication module cannot be loaded");

            ***/

            if (string.IsNullOrEmpty(_qqExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_qqExternalAuthSettings.ClientSecret))
                throw new NopException("QQ authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "QQAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString("ErrorCallback", Url.RouteUrl("Login", new { returnUrl }));

            return Challenge(authenticationProperties, QQDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate QQ user
            var authenticateResult =  await HttpContext.AuthenticateAsync(QQDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = QQAuthenticationDefaults.ProviderSystemName,
                AccessToken = await HttpContext.GetTokenAsync(QQDefaults.AuthenticationScheme, "access_token"),
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