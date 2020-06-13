using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.WeChatPay
{
    public static partial class WeChatPayDefaults
    {
        /// <summary>
        /// 扫描页面的路由名
        /// </summary>
        public static string ScanCodeRouteName => "Plugin.Payments.WeChatPay.WeChatScanCode";

        /// <summary>
        /// 生成QR code 的路由名
        /// </summary>
        public static string GenerateQRCodeRouteName => "Plugin.Payments.WeChatPay.MakeQRCode";
        /// <summary>
        /// 支付结果回调路由名
        /// </summary>
        public static string NotifyRouteName => "Plugin.Payments.WeChatPay.Notify";
        // Plugins/PaymentWeChatPay/RefundNotify
        public static string NotifyUrl => "Plugins/PaymentWeChatPay/Notify";

        /// <summary>
        /// 退款结果回调路由名
        /// </summary>
        public static string RefundNotifyRouteName => "Plugin.Payments.WeChatPay.RefundNotify";
        public static string RefundNotifyUrl => "Plugins/PaymentWeChatPay/RefundNotify";
        /// <summary>
        /// 支付结束，处理返回商户网站的路由名
        /// </summary>
        public static string ReturnRouteName => "Plugin.Payments.WeChatPay.Return";

        public static string TradeType => "NATIVE";
        
    }
    public static partial class WeChatTradeTypeDefaults
    {
        /// <summary>
        /// 扫描页面的路由名
        /// </summary>
        public static string MobileWeb => "MWEB";

        /// <summary>
        /// 生成QR code 的路由名
        /// </summary>
        public static string Native => "NATIVE";
        /// <summary>
        /// 支付结果回调路由名
        /// </summary>
        public static string JsApi => "JSAPI";
        /// <summary>
        /// 支付结束，处理返回商户网站的路由名
        /// </summary>
        public static string App => "APP";

    }
}
