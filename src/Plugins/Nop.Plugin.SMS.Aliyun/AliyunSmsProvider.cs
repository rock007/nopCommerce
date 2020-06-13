using Nop.Core;
using Nop.Services.Plugins;
using Nop.Plugin.SMS.Aliyun.Data;
using Nop.Plugin.SMS.Aliyun.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.SMS.Aliyun
{
    /// <summary>
    /// 阿里云短信发送服务插件
    /// </summary>
    public partial class AliyunSmsProvider : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly AliyunSmsSettings _smsSettings;
        //private readonly IPriceCalculationService _priceCalculationService;
        //private readonly IProductAttributeParser _productAttributeParser;
        //private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        //private readonly IQueuedSmsService _shippingByWeightService;
        //private readonly IShippingService _shippingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly AliyunSmsObjectContext _objectContext;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public AliyunSmsProvider(AliyunSmsSettings smsSettings,
            //IPriceCalculationService priceCalculationService,
            //IProductAttributeParser productAttributeParser,
            //IProductService productService,
            ISettingService settingService,
            //IShippingByWeightService shippingByWeightService,
            //IShippingService shippingService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IWebHelper webHelper,
            AliyunSmsObjectContext objectContext,
            ILocalizationService localizationService,
            ILogger logger)
        {
            this._smsSettings = smsSettings;
            this._settingService = settingService;
            //this._priceCalculationService = priceCalculationService;
            //this._productAttributeParser = productAttributeParser;
            //this._productService = productService;
            //this._settingService = settingService;
            //this._shippingByWeightService = shippingByWeightService;
            //this._shippingService = shippingService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._objectContext = objectContext;
            this._logger = logger;
            this._localizationService = localizationService;
        }

        #endregion

        #region Utilities


        #endregion

        #region IAdminMenuPlugin


        public void ManageSiteMap(SiteMapNode rootNode)
        {
            string pluginMenuName = _localizationService.GetResource("Plugins.SMS.Aliyun.Menu.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "Short Messages");
            string settingsMenuName = _localizationService.GetResource("Plugins.SMS.Aliyun.Menu.Settings.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "Short Messages Settings");
            string manageTemplatesMenuName = _localizationService.GetResource("Plugins.SMS.Aliyun.Menu.Clients.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "Template");

            var pluginMainMenu = new SiteMapNode
            {
                Title = pluginMenuName,
                Visible = true,
                SystemName = "Manage Short Message Menu",
                IconClass = "fa-genderless"
            };

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = settingsMenuName,
                Url = _webHelper.GetStoreLocation() + AreaNames.Admin + "/AliyunSms/Configure",
                Visible = true,
                SystemName = "Short Message Settings",
                IconClass = "fa-genderless"
            });

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = manageTemplatesMenuName,
                Url = _webHelper.GetStoreLocation() + AreaNames.Admin + "/SmsTemplate/List",
                Visible = true,
                SystemName = "Manage Short Message Template",
                IconClass = "fa-genderless"
            });
            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = _localizationService.GetResource("Plugins.SMS.Aliyun.Menu.QueuedSms.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "Manage Short Message"),
                Url = _webHelper.GetStoreLocation() + AreaNames.Admin + "/QueuedSms/List",
                Visible = true,
                SystemName = "Manage Short Message",
                IconClass = "fa-genderless"
            });

            string pluginDocumentationUrl = "https://shop117119354.taobao.com";

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = _localizationService.GetResource("Plugins.SMS.Aliyun.Menu.Docs.Title"),
                Url = pluginDocumentationUrl,
                Visible = true,
                SystemName = "SMS-Aliyun-Docs-Menu",
                IconClass = "fa-genderless"
            });//TODO: target="_blank"


            rootNode.ChildNodes.Add(pluginMainMenu);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get { return ShippingRateComputationMethodType.Offline; }
        }
        
        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get
            {
                //uncomment a line below to return a general shipment tracker (finds an appropriate tracker by tracking number)
                //return new GeneralShipmentTracker(EngineContext.Current.Resolve<ITypeFinder>());
                return null;
            }
        }

        #endregion
    }
}
