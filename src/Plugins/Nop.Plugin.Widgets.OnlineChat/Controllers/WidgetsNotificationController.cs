using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Notification.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Notification.Controllers
{
    [Area(AreaNames.Admin)]
    public class WidgetsNotificationController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsNotificationController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService, 
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            this._localizationService = localizationService;
            this._notificationService = notificationService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._storeContext = storeContext;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var notificationSettings = _settingService.LoadSetting<NotificationSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Picture1Id = notificationSettings.Picture1Id,
                Text1 = notificationSettings.Text1,
                Link1 = notificationSettings.Link1,
                AltText1 = notificationSettings.AltText1,
                Picture2Id = notificationSettings.Picture2Id,
                Text2 = notificationSettings.Text2,
                Link2 = notificationSettings.Link2,
                AltText2 = notificationSettings.AltText2,
                Picture3Id = notificationSettings.Picture3Id,
                Text3 = notificationSettings.Text3,
                Link3 = notificationSettings.Link3,
                AltText3 = notificationSettings.AltText3,
                Picture4Id = notificationSettings.Picture4Id,
                Text4 = notificationSettings.Text4,
                Link4 = notificationSettings.Link4,
                AltText4 = notificationSettings.AltText4,
                Picture5Id = notificationSettings.Picture5Id,
                Text5 = notificationSettings.Text5,
                Link5 = notificationSettings.Link5,
                AltText5 = notificationSettings.AltText5,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Picture1Id_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Picture1Id, storeScope);
                model.Text1_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Text1, storeScope);
                model.Link1_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Link1, storeScope);
                model.AltText1_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.AltText1, storeScope);
                model.Picture2Id_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Picture2Id, storeScope);
                model.Text2_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Text2, storeScope);
                model.Link2_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Link2, storeScope);
                model.AltText2_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.AltText2, storeScope);
                model.Picture3Id_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Picture3Id, storeScope);
                model.Text3_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Text3, storeScope);
                model.Link3_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Link3, storeScope);
                model.AltText3_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.AltText3, storeScope);
                model.Picture4Id_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Picture4Id, storeScope);
                model.Text4_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Text4, storeScope);
                model.Link4_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Link4, storeScope);
                model.AltText4_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.AltText4, storeScope);
                model.Picture5Id_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Picture5Id, storeScope);
                model.Text5_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Text5, storeScope);
                model.Link5_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.Link5, storeScope);
                model.AltText5_OverrideForStore = _settingService.SettingExists(notificationSettings, x => x.AltText5, storeScope);
            }

            return View("~/Plugins/Widgets.Notification/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var notificationSettings = _settingService.LoadSetting<NotificationSettings>(storeScope);

            //get previous picture identifiers
            var previousPictureIds = new[] 
            {
                notificationSettings.Picture1Id,
                notificationSettings.Picture2Id,
                notificationSettings.Picture3Id,
                notificationSettings.Picture4Id,
                notificationSettings.Picture5Id
            };

            notificationSettings.Picture1Id = model.Picture1Id;
            notificationSettings.Text1 = model.Text1;
            notificationSettings.Link1 = model.Link1;
            notificationSettings.AltText1 = model.AltText1;
            notificationSettings.Picture2Id = model.Picture2Id;
            notificationSettings.Text2 = model.Text2;
            notificationSettings.Link2 = model.Link2;
            notificationSettings.AltText2 = model.AltText2;
            notificationSettings.Picture3Id = model.Picture3Id;
            notificationSettings.Text3 = model.Text3;
            notificationSettings.Link3 = model.Link3;
            notificationSettings.AltText3 = model.AltText3;
            notificationSettings.Picture4Id = model.Picture4Id;
            notificationSettings.Text4 = model.Text4;
            notificationSettings.Link4 = model.Link4;
            notificationSettings.AltText4 = model.AltText4;
            notificationSettings.Picture5Id = model.Picture5Id;
            notificationSettings.Text5 = model.Text5;
            notificationSettings.Link5 = model.Link5;
            notificationSettings.AltText5 = model.AltText5;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Picture1Id, model.Picture1Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Text1, model.Text1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Link1, model.Link1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.AltText1, model.AltText1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Picture2Id, model.Picture2Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Text2, model.Text2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Link2, model.Link2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.AltText2, model.AltText2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Picture3Id, model.Picture3Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Text3, model.Text3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Link3, model.Link3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.AltText3, model.AltText3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Picture4Id, model.Picture4Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Text4, model.Text4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Link4, model.Link4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.AltText4, model.AltText4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Picture5Id, model.Picture5Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Text5, model.Text5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.Link5, model.Link5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(notificationSettings, x => x.AltText5, model.AltText5_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();
            
            //get current picture identifiers
            var currentPictureIds = new[]
            {
                notificationSettings.Picture1Id,
                notificationSettings.Picture2Id,
                notificationSettings.Picture3Id,
                notificationSettings.Picture4Id,
                notificationSettings.Picture5Id
            };

            //delete an old picture (if deleted or updated)
            foreach (var pictureId in previousPictureIds.Except(currentPictureIds))
            { 
                var previousPicture = _pictureService.GetPictureById(pictureId);
                if (previousPicture != null)
                    _pictureService.DeletePicture(previousPicture);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}