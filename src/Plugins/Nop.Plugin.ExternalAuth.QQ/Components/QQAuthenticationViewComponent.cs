using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.ExternalAuth.QQ;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.Facebook.Components
{
    [ViewComponent(Name = QQAuthenticationDefaults.ViewComponentName)]
    public class QQAuthenticationViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ExternalAuth.QQ/Views/PublicInfo.cshtml");
        }
    }
}