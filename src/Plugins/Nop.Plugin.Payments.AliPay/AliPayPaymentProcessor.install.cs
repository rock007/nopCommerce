using Essensoft.AspNetCore.Payment.Alipay.Request;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Nop.Plugin.Payments.AliPay
{
    /// <summary>
    /// 支付宝支付，支持当面付、手机网站支付、电脑网站支付,
    ///  
    /// </summary>
    public partial class AliPayPaymentProcessor
    {

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new AliPayPaymentSettings
            {
                AdditionalFeePercentage = true,
                AdditionalFee = 0,
                LogLevel = Microsoft.Extensions.Logging.LogLevel.Error
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.ApplicationId", "Application Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.ApplicationId.Hint", "Application Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.SignType", "签名方式");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.SignType.Hint", "签名方式");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.RsaPublicKey", "支付宝公钥");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.RsaPublicKey.Hint", "支付宝公钥");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.RsaPrivateKey", "应用私钥");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.RsaPrivateKey.Hint", "应用私钥");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.EncyptKey", "EncyptKey");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.EncyptKey.Hint", "EncyptKey");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.ServerUrl", "支付宝网关");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.ServerUrl.Hint", "这里要注意沙箱环境的网关地址为：https://openapi.alipaydev.com/gateway.do");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Version", "接口版本");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Version.Hint", "接口版本");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Format", "数据格式");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Format.Hint", "数据格式");
            
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Charset", "编码格式");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Charset.Hint", "编码格式");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.EncyptType", "加密方式");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.EncyptType.Hint", "加密方式");
      


            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFee", "附加费");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFee.Hint", "附加费");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFeePercentage", "附加费比例");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFeePercentage.Hint", "附加费比例");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.Instructions", "支付宝支付");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.ScanQRCode", "确认订单后，会出现支付二维码，请使用支付宝扫一扫功能，进行支付。");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.RedirectionTip", "确认订单后，将打开支付宝在线支付页面，进行支付。");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.AliPay.PaymentMethodDescription", "支付宝支付，支持当面付、手机网站支付、电脑网站支付");
            //  paymentmethoddescription

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<AliPayPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.ApplicationId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.ApplicationId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.RsaPublicKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.RsaPublicKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.RsaPrivateKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.RsaPrivateKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.EncyptKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.EncyptKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.ServerUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.ServerUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Version");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Version.Hint");



            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.SignType");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.SignType.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Format");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Format.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Charset");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Charset.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.EncyptType");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.EncyptType.Hint");


            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.ScanQRCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.RedirectionTip");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.AliPay.PaymentMethodDescription");

            base.Uninstall();
        }
    }
}
