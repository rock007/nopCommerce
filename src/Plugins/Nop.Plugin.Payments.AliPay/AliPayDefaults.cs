using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.AliPay
{
    public static partial class AliPayDefaults
    {
        /// <summary>
        /// 扫描页面的路由名
        /// </summary>
        public const string ScanCodeRouteName = "Plugin.Payments.AliPay.AliPayScanCode";

        /// <summary>
        /// 生成QR code 的路由名
        /// </summary>
        public const string GenerateQRCodeRouteName = "Plugin.Payments.AliPay.MakeQRCode";
        /// <summary>
        /// 扫码支付异步通知回调路由名
        /// </summary>
        public const string PrecreateNotifyRouteName = "Plugin.Payments.AliPay.PrecreateNotify";
        /// <summary>
        /// 电脑网站支付异步通知回调路由名
        /// </summary>
        public const string PagePayNotifyRouteName = "Plugin.Payments.AliPay.PagePayNotify";
        /// <summary>
        /// 手机网站支付异步通知回调路由名
        /// </summary>
        public const string WapPayNotifyRouteName = "Plugin.Payments.AliPay.WapPayNotify";
        /// <summary>
        /// 支付结束，处理返回商户网站的路由名
        /// </summary>
        public const string ReturnRouteName = "Plugin.Payments.AliPay.Return";

        public const string TradeType = "NATIVE";

        public const string ProductCode = "FAST_INSTANT_TRADE_PAY";

        // ProductCode = "FAST_INSTANT_TRADE_PAY"

    }
}
