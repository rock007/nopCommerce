using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.WeChatPay.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.WeChatPay.Components
{
    [ViewComponent(Name = "PaymentWeChatPay")]
    public class PaymentWeChatPayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel();
            return View("~/Plugins/Payments.WeChatPay/Views/PaymentInfo.cshtml", model);
        }
    }
}
