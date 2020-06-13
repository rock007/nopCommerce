using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.QQ
{
    /// <summary>
    /// Represents method for the authentication with QQ account
    /// </summary>
    public class QQAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public QQAuthenticationMethod(ISettingService settingService,
            ILocalizationService localizationService,
            IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/QQAuthentication/Configure";
        }

        public string GetPublicViewComponentName()
        {
            return QQAuthenticationDefaults.ViewComponentName;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new QQExternalAuthSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientKeyIdentifier", "App ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your QQ application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientSecret", "App Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientSecret.Hint", "Enter your app secret here. You can find it on your QQ application page.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<QQExternalAuthSettings>();

            //locales
             _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientKeyIdentifier");
             _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientKeyIdentifier.Hint");
             _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientSecret");
             _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.QQ.ClientSecret.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
