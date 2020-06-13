using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.WeChatPay.Models;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.WeChatPay.Controllers
{
    public partial class PaymentWeChatPayController
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
            var weChatPaymentSettings = _settingService.LoadSetting<WeChatPayPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                AdditionalFee = weChatPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = weChatPaymentSettings.AdditionalFeePercentage,
                AppId = weChatPaymentSettings.AppId,
                AppSecret = weChatPaymentSettings.AppSecret,
                MerchantId = weChatPaymentSettings.MerchantId,
                Key = weChatPaymentSettings.Key,
                SslCertificationPath = weChatPaymentSettings.SslCertificationPath,
                SslCertificationPassword = weChatPaymentSettings.SslCertificationPassword,
                LogLevel = weChatPaymentSettings.LogLevel,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.AppId_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.AppId, storeScope);
                model.AppSecret_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.AppSecret, storeScope);
                model.MerchantId_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.MerchantId, storeScope);
                model.Key_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.Key, storeScope);
                model.SslCertificationPath_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.SslCertificationPath, storeScope);
                model.SslCertificationPassword_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.SslCertificationPassword, storeScope);

                model.LogLevel_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.LogLevel, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(weChatPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.WeChatPay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var weChatPaymentSettings = _settingService.LoadSetting<WeChatPayPaymentSettings>(storeScope);

            //save settings
            weChatPaymentSettings.AppId = model.AppId;
            weChatPaymentSettings.AppSecret = model.AppSecret;
            weChatPaymentSettings.MerchantId = model.MerchantId;
            weChatPaymentSettings.Key = model.Key;
            weChatPaymentSettings.SslCertificationPath = model.SslCertificationPath;
            weChatPaymentSettings.SslCertificationPassword = model.SslCertificationPassword;
            weChatPaymentSettings.LogLevel = model.LogLevel;
            weChatPaymentSettings.AdditionalFee = model.AdditionalFee;
            weChatPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.AppId, model.AppId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.AppSecret, model.AppSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.Key, model.Key_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.SslCertificationPath, model.SslCertificationPath_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.SslCertificationPassword, model.SslCertificationPassword_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.LogLevel, model.LogLevel_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(weChatPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}