using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AliPay.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.AliPay.Components
{
    [ViewComponent(Name = "PaymentAliPay")]
    public class PaymentAliPayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel();
            return View("~/Plugins/Payments.AliPay/Views/PaymentInfo.cshtml", model);
        }
    }
}