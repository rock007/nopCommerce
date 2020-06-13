using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Notification
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class NotificationPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly INopFileProvider _fileProvider;

        public NotificationPlugin(ILocalizationService localizationService,
            IPictureService pictureService,
            ISettingService settingService,
            IWebHelper webHelper,
            INopFileProvider fileProvider)
        {
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._fileProvider = fileProvider;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "page-bottom" };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsNotification/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsNotification";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            ////pictures
            //var sampleImagesPath = _fileProvider.MapPath("~/Plugins/Widgets.Notification/Content/nivoslider/sample-images/");

            ////settings
            //var settings = new NotificationSettings
            //{
            //    Picture1Id = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(sampleImagesPath + "banner1.jpg"), MimeTypes.ImagePJpeg, "banner_1").Id,
            //    Text1 = "",
            //    Link1 = _webHelper.GetStoreLocation(false),
            //    Picture2Id = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(sampleImagesPath + "banner2.jpg"), MimeTypes.ImagePJpeg, "banner_2").Id,
            //    Text2 = "",
            //    Link2 = _webHelper.GetStoreLocation(false)
            //    //Picture3Id = _pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner3.jpg"), MimeTypes.ImagePJpeg, "banner_3").Id,
            //    //Text3 = "",
            //    //Link3 = _webHelper.GetStoreLocation(false),
            //};
            //_settingService.SaveSetting(settings);


            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture1", "Picture 1");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture2", "Picture 2");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture3", "Picture 3");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture4", "Picture 4");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture5", "Picture 5");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture", "Picture");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Picture.Hint", "Upload picture.");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Text", "Comment");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Link", "URL");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.AltText", "Image alternate text");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Notification.AltText.Hint", "Enter alternate text that will be added to image.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            ////settings
            //_settingService.DeleteSetting<NotificationSettings>();

            ////locales
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture1");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture2");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture3");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture4");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture5");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Picture.Hint");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Text");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Text.Hint");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Link");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.Link.Hint");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.AltText");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.Notification.AltText.Hint");

            base.Uninstall();
        }
    }
}