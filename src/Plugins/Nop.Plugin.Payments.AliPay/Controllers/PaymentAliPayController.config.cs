using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AliPay.Models;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.AliPay.Controllers
{
    public partial class PaymentAliPayController
    {
        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var aliPayPaymentSettings = _settingService.LoadSetting<AliPayPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                AdditionalFee = aliPayPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = aliPayPaymentSettings.AdditionalFeePercentage,
                ApplicationId = aliPayPaymentSettings.AppId,
                RsaPublicKey = aliPayPaymentSettings.RsaPublicKey,
                RsaPrivateKey = aliPayPaymentSettings.RsaPrivateKey,
                EncyptKey = aliPayPaymentSettings.EncyptKey,
                LogLevel = aliPayPaymentSettings.LogLevel,

                Charset = aliPayPaymentSettings.Charset,
                EncyptType = aliPayPaymentSettings.EncyptType,
                Format = aliPayPaymentSettings.Format,
                //  ReportLevel = aliPayPaymentSettings.r
                ServerUrl = aliPayPaymentSettings.ServerUrl,
                SignType = aliPayPaymentSettings.SignType,
                Version = aliPayPaymentSettings.Version,

                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.ApplicationId_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.AppId, storeScope);
                model.RsaPublicKey_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.RsaPublicKey, storeScope);
                model.RsaPrivateKey_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.RsaPrivateKey, storeScope);
                model.EncyptKey_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.EncyptKey, storeScope);
                model.LogLevel_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.LogLevel, storeScope);

                model.Charset_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.Charset, storeScope);
                model.EncyptType_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.EncyptType, storeScope);
                model.Format_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.Format, storeScope);
                model.ServerUrl_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.ServerUrl, storeScope);
                model.SignType_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.SignType, storeScope);
                model.Version_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.Version, storeScope);

                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(aliPayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.AliPay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
       // [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var aliPayPaymentSettings = _settingService.LoadSetting<AliPayPaymentSettings>(storeScope);

            //save settings
            aliPayPaymentSettings.AppId = model.ApplicationId;
            aliPayPaymentSettings.RsaPublicKey = model.RsaPublicKey;
            aliPayPaymentSettings.RsaPrivateKey = model.RsaPrivateKey;
            aliPayPaymentSettings.EncyptKey = model.EncyptKey;
            aliPayPaymentSettings.LogLevel = model.LogLevel;

            //aliPayPaymentSettings.Charset = model.Charset;
            //aliPayPaymentSettings.EncyptType = model.EncyptType;
            aliPayPaymentSettings.Format = model.Format;
            aliPayPaymentSettings.ServerUrl = model.ServerUrl;
            aliPayPaymentSettings.SignType = model.SignType;
            aliPayPaymentSettings.Version = model.Version;

            aliPayPaymentSettings.AdditionalFee = model.AdditionalFee;
            aliPayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.AppId, model.ApplicationId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.RsaPublicKey, model.RsaPublicKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.RsaPrivateKey, model.RsaPrivateKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.EncyptKey, model.EncyptKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.LogLevel, model.LogLevel_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.Format, model.Format_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.ServerUrl, model.ServerUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.SignType, model.SignType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.Version, model.Version_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(aliPayPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion Methods
    }
}