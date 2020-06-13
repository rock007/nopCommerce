using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Notification.Infrastructure.Cache;
using Nop.Plugin.Widgets.Notification.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Notification.Components
{
    [ViewComponent(Name = "WidgetsNotification")]
    public class WidgetsNotificationViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;

        public WidgetsNotificationViewComponent(IStoreContext storeContext, 
            IStaticCacheManager cacheManager, 
            ISettingService settingService, 
            IPictureService pictureService,
            IWebHelper webHelper)
        {
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
            this._settingService = settingService;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var notificationSettings = _settingService.LoadSetting<NotificationSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Picture1Url = GetPictureUrl(notificationSettings.Picture1Id),
                Text1 = notificationSettings.Text1,
                Link1 = notificationSettings.Link1,
                AltText1 = notificationSettings.AltText1,

                Picture2Url = GetPictureUrl(notificationSettings.Picture2Id),
                Text2 = notificationSettings.Text2,
                Link2 = notificationSettings.Link2,
                AltText2 = notificationSettings.AltText2,

                Picture3Url = GetPictureUrl(notificationSettings.Picture3Id),
                Text3 = notificationSettings.Text3,
                Link3 = notificationSettings.Link3,
                AltText3 = notificationSettings.AltText3,

                Picture4Url = GetPictureUrl(notificationSettings.Picture4Id),
                Text4 = notificationSettings.Text4,
                Link4 = notificationSettings.Link4,
                AltText4 = notificationSettings.AltText4,

                Picture5Url = GetPictureUrl(notificationSettings.Picture5Id),
                Text5 = notificationSettings.Text5,
                Link5 = notificationSettings.Link5,
                AltText5 = notificationSettings.AltText5
            };

            //if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
            //    string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
            //    string.IsNullOrEmpty(model.Picture5Url))
            //    //no pictures uploaded
            //    return Content("");

            return View("~/Plugins/Widgets.Notification/Views/PublicInfo.cshtml", model);
        }

        protected string GetPictureUrl(int pictureId)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, 
                pictureId, _webHelper.IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp);

            return _cacheManager.Get(cacheKey, () =>
            {
                //little hack here. nulls aren't cacheable so set it to ""
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false) ?? "";
                return url;
            });
        }
    }
}
