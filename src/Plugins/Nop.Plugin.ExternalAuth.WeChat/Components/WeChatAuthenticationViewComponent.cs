using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.WeChat.Components
{
    [ViewComponent(Name = WeChatAuthenticationDefaults.ViewComponentName)]
    public class WeChatAuthenticationViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ExternalAuth.WeChat/Views/PublicInfo.cshtml");
        }
    }
}