using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.WeChat
{
    /// <summary>
    /// Represents method for the authentication with WeChat account
    /// </summary>
    public class WeChatAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public WeChatAuthenticationMethod(ISettingService settingService,
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
            return $"{_webHelper.GetStoreLocation()}Admin/WeChatAuthentication/Configure";
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store
        /// </summary>
        public string GetPublicViewComponentName()
        {
            return WeChatAuthenticationDefaults.ViewComponentName;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new WeChatExternalAuthSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientKeyIdentifier", "App ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your WeChat application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientSecret", "App Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientSecret.Hint", "Enter your app secret here. You can find it on your WeChat application page.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WeChatExternalAuthSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientKeyIdentifier");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientKeyIdentifier.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.WeChat.ClientSecret.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
