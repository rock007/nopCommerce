using Nop.Services.Configuration;
using System;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Nop.Services.Logging;

namespace Nop.Plugin.SMS.Aliyun.Services
{
    public partial class AliyunSmsSender : ISmsSender
    {
        #region Fields

        private readonly AliyunSmsSettings _smsSettings;
        //private readonly IPriceCalculationService _priceCalculationService;
        //private readonly IProductAttributeParser _productAttributeParser;
        //private readonly IProductService _productService;
        //private readonly ISettingService _settingService;
        ////private readonly IQueuedSmsService _shippingByWeightService;
        //private readonly IShippingService _shippingService;
        //private readonly IWorkContext _workContext;
        //private readonly IStoreContext _storeContext;
        //private readonly IWebHelper _webHelper;
        //private readonly AliyunSmsObjectContext _objectContext;
        private readonly ILogger _logger;
        //private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public AliyunSmsSender(AliyunSmsSettings smsSettings,
            //IPriceCalculationService priceCalculationService,
            //IProductAttributeParser productAttributeParser,
            //IProductService productService,
            //ISettingService settingService,
            ////IShippingByWeightService shippingByWeightService,
            ////IShippingService shippingService,
            //IWorkContext workContext,
            //IStoreContext storeContext,
            //IWebHelper webHelper,
            //AliyunSmsObjectContext objectContext,
            //ILocalizationService localizationService,
            ILogger logger)
        {
            this._smsSettings = smsSettings;
            //this._settingService = settingService;
            //this._priceCalculationService = priceCalculationService;
            //this._productAttributeParser = productAttributeParser;
            //this._productService = productService;
            //this._settingService = settingService;
            //this._shippingByWeightService = shippingByWeightService;
            //this._shippingService = shippingService;
            //this._workContext = workContext;
            //this._storeContext = storeContext;
            //this._webHelper = webHelper;
            //this._objectContext = objectContext;
            this._logger = logger;
            //this._localizationService = localizationService;
        }

        #endregion

        public bool SendSms(string phoneNumbers, string message)
        {
            throw new NotImplementedException();
        }

        public bool SendSms(string phoneNumbers, string templateCode, string templateParams = null, string outId = null)
        {
            var returnValue = false;
            //String product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            //String domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            //String accessKeyId = _smsSettings.AccessKeyID;//你的accessKeyId，参考本文档步骤2
            //String accessKeySecret = _smsSettings.AccessKeySecret;//你的accessKeySecret，参考本文档步骤2
            IClientProfile profile = DefaultProfile.GetProfile(_smsSettings.RegionId, _smsSettings.AccessKeyID, _smsSettings.AccessKeySecret);
            // IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();
            //初始化ascClient,暂时不支持多region（请勿修改）
            DefaultProfile.AddEndpoint(_smsSettings.EndpointName,
                _smsSettings.RegionId,
                _smsSettings.Product,
                _smsSettings.Domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phoneNumbers;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = _smsSettings.SignName;
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = templateCode;// "SMS_00000001";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = templateParams;// "{\"customer\":\"123\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = outId;// "yourOutId";
                //请求失败这里会抛ClientException异常
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                //System.Console.WriteLine(sendSmsResponse.Message);
                if (sendSmsResponse.Code == "OK")
                {
                    returnValue = true;
                }
                else
                {
                    _logger.Error(sendSmsResponse.Message);
                }
            }
            catch (ServerException e)
            {
                _logger.Error("短信发送失败：" + e.ErrorMessage + ", 错误类型：" + e.ErrorType + ", 错误编号：" + e.ErrorCode + ", " + e.Data, e);
                // System.Console.WriteLine("Hello World!");
                returnValue = false;
            }
            catch (ClientException e)
            {
                //System.Console.WriteLine("Hello World!");
                _logger.Error("短信发送失败：" + e.ErrorMessage + ", 错误类型：" + e.ErrorType + ", 错误编号：" + e.ErrorCode + ", " + e.Data, e);
                returnValue = false;
            }
            return returnValue;
        }

    }
}
