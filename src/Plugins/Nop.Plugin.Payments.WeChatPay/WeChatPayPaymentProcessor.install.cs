using Essensoft.AspNetCore.Payment.WeChatPay.Request;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Nop.Plugin.Payments.WeChatPay
{
    public partial class WeChatPayPaymentProcessor
    {

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new WeChatPayPaymentSettings
            {
                AdditionalFeePercentage = true,
                AdditionalFee = 0,
                LogLevel = Microsoft.Extensions.Logging.LogLevel.Error
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AppId", "微信支付AppId");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AppId.Hint", "微信支付AppId");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AppSecret", "公众帐号secert");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AppSecret.Hint", "公众帐号secert");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.MerchantId", "商户号");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.MerchantId.Hint", "商户号");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.Key", "商户支付密钥");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.Key.Hint", "商户支付密钥");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFee", "附加费");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFee.Hint", "附加费");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFeePercentage", "附加费比例");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFeePercentage.Hint", "附加费比例");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPath", "证书文件路径");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPath.Hint", "证书文件路径");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPassword", "证书文件密码");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPassword.Hint", "证书文件密码");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.Instructions", "微信扫码支付");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.ScanQRCode", "确认订单后，会出现支付二维码，请使用我微信扫一扫功能，进行支付。");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.RedirectionTip", "确认订单后，将打开微信在线支付页面，进行支付。");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.WeChatPay.PaymentMethodDescription", "微信支付，支持付款码、Native、H5支付");
            //  paymentmethoddescription

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WeChatPayPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AppId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AppId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AppSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AppSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.MerchantId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.MerchantId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.Key");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.Key.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPath");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPath.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPassword");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.SslCertificationPassword.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.ScanQRCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.RedirectionTip");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.WeChatPay.PaymentMethodDescription");

            base.Uninstall();
        }
    }
}
