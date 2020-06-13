using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Plugin.ExternalAuth.QQ.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.QQ.Controllers
{
    public partial class QQAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly QQExternalAuthSettings _qqExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IOptionsMonitorCache<QQOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public QQAuthenticationController(QQExternalAuthSettings qqExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOptionsMonitorCache<QQOptions> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            this._qqExternalAuthSettings = qqExternalAuthSettings;
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
                ClientId = _qqExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _qqExternalAuthSettings.ClientSecret
            };

            return View("~/Plugins/ExternalAuth.QQ/Views/Configure.cshtml", model);
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
            _qqExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _qqExternalAuthSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_qqExternalAuthSettings);

            //clear QQ authentication options cache
            _optionsCache.TryRemove(QQDefaults.AuthenticationScheme);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }


        #endregion
    }
}