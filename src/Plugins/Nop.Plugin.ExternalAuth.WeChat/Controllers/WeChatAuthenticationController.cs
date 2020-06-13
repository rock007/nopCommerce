using Microsoft.AspNetCore.Authentication.WeChat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Plugin.ExternalAuth.WeChat.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.WeChat.Controllers
{
    public partial class WeChatAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly WeChatExternalAuthSettings _wechatExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IOptionsMonitorCache<WeChatOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public WeChatAuthenticationController(WeChatExternalAuthSettings wechatExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOptionsMonitorCache<WeChatOptions> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            this._wechatExternalAuthSettings = wechatExternalAuthSettings;
            this._externalAuthenticationService = externalAuthenticationService;
            this._localizationService = localizationService;
            this._optionsCache = optionsCache;
            this._permissionService = permissionService;
            this._settingService = settingService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _wechatExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _wechatExternalAuthSettings.ClientSecret
            };

            return View("~/Plugins/ExternalAuth.WeChat/Views/Configure.cshtml", model);
        }

        [HttpPost]
        //[AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _wechatExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _wechatExternalAuthSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_wechatExternalAuthSettings);

            //clear WeChat authentication options cache
            _optionsCache.TryRemove(WeChatDefaults.AuthenticationScheme);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}