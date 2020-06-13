using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.AliPay
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapControllerRoute(AliPayDefaults.ScanCodeRouteName,
                "Plugins/PaymentAliPay/AliPayScanCode/{ordercode}",
                new { controller = "PaymentAliPay", action = "AliPayScanCode" }
           //new[] { "Nop.Plugin.Payments.AliPay.Controllers" }
           );
            //MakePayQRCode
            routeBuilder.MapControllerRoute(AliPayDefaults.GenerateQRCodeRouteName,
                 "Plugins/PaymentAliPay/MakeQRCode",
                 new { controller = "PaymentAliPay", action = "MakeQRCode" }
                 //new[] { "Nop.Plugin.Payments.AliPay.Controllers" }
            );

            //扫码支付异步通知回调路由名
            routeBuilder.MapControllerRoute(AliPayDefaults.PrecreateNotifyRouteName,
                 "Plugins/PaymentAliPay/Notify",
                 new { controller = "PaymentAliPay", action = "PrecreateNotify" }
                 //new[] { "Nop.Plugin.Payments.AliPay.Controllers" }
            );
            //电脑网站支付异步通知回调路由名
            routeBuilder.MapControllerRoute(AliPayDefaults.PagePayNotifyRouteName,
                 "Plugins/PaymentAliPay/Notify",
                 new { controller = "PaymentAliPay", action = "PagePayNotify" }
                 //new[] { "Nop.Plugin.Payments.AliPay.Controllers" }
            );
            //手机网站支付异步通知回调路由名
            routeBuilder.MapControllerRoute(AliPayDefaults.WapPayNotifyRouteName,
                 "Plugins/PaymentAliPay/Notify",
                 new { controller = "PaymentAliPay", action = "WapPayNotify" }
                 //new[] { "Nop.Plugin.Payments.AliPay.Controllers" }
            );

            //Return 支付结束，处理返回商户网站的路由名
            routeBuilder.MapControllerRoute(AliPayDefaults.ReturnRouteName,
                 "Plugins/PaymentAliPay/Return",
                 new { controller = "PaymentAliPay", action = "Return" }
                 //new[] { "Nop.Plugin.Payments.AliPay.Controllers" }
            );
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return 0; }
        }
    }
}
