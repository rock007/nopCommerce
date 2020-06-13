using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Nop.Core;
using Nop.Services.Plugins;
using Nop.Plugin.SMS.Aliyun.Data;
using Nop.Plugin.SMS.Aliyun.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using System;

namespace Nop.Plugin.SMS.Aliyun
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public partial class AliyunSmsProvider
    {
        #region BasePlugin

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AliyunSms/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new AliyunSmsSettings() {
                Domain = "dysmsapi.aliyuncs.com",
                Enabled = true,
                EndpointName = "cn-hangzhou",
                Product = "Dysmsapi",
                RegionId = "cn-hangzhou"
            });

            //database objects
            _objectContext.Install();

            //locales
            // 
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun", "Aliyun Short Message services");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Menu.Docs.Title", "技术咨询");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Common.NoAuthorize", "您无权执行此操作");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Test.Failure", "发送短信失败，请检查日志");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.SmsTemplates.NoExisted", "短信模板不存在，请先创建并保存相应模板");
            // 
            //Action without authorization   

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.List", "短信模板列表");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.AddNew", "新建短信模板");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Edit", "编辑短信模板");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Copy", "复制短信模板");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.List", "短信列表");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.AddNew", "新建短信");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Edit", "编辑短信");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Copy", "复制短信");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Enabled", "Enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Enabled.Hint", "Enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.AccessKeyID", "AccessKeyID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.AccessKeyID.Hint", "AccessKeyID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.AccessKeySecret", "AccessKeySecret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.AccessKeySecret.Hint", "AccessKeySecret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.SignName", "SignName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.SignName.Hint", "SignName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Product", "Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Product.Hint", "Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Domain", "Domain");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Domain.Hint", "Domain");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.RegionId", "RegionId");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.RegionId.Hint", "RegionId");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.EndpointName", "EndpointName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.EndpointName.Hint", "EndpointName");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.SystemName", "SystemName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.SystemName.Hint", "SystemName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.TemplateCode", "TemplateCode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.TemplateCode.Hint", "TemplateCode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.Subject", "Subject");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.Subject.Hint", "Subject");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.Body", "Body");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.Body.Hint", "Body");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.IsActive", "IsActive");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.IsActive.Hint", "IsActive");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.SendImmediately", "SendImmediately");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.SendImmediately.Hint", "SendImmediately");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.DelayBeforeSend", "DelayBeforeSend");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.DelayBeforeSend.Hint", "DelayBeforeSend");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Priority", "Priority");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Priority.Hint", "Priority");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.TemplateCode", "TemplateCode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.TemplateCode.Hint", "TemplateCode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Subject", "Subject");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Subject.Hint", "Subject");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Body", "Body");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Body.Hint", "Body");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.ToMobileNumber", "ToMobileNumber");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.ToMobileNumber.Hint", "ToMobileNumber");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SendImmediately", "SendImmediately");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SendImmediately.Hint", "SendImmediately");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.ToName", "ToName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.ToName.Hint", "ToName");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.TemplateParamJson", "TemplateParamJson");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.TemplateParamJson.Hint", "TemplateParamJson");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.CreatedOn", "CreatedOn");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.CreatedOn.Hint", "CreatedOn");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.DontSendBeforeDate", "DontSendBeforeDate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.DontSendBeforeDate.Hint", "DontSendBeforeDate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SentTries", "SentTries");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SentTries.Hint", "SentTries");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SentOn", "SentOn");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SentOn.Hint", "SentOn");

            base.Install();
        }
        
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<AliyunSmsSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales

            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Menu.Docs.Title");
            _localizationService.DeletePluginLocaleResource("Admin.Common.NoAuthorize");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Test.Failure");
            _localizationService.DeletePluginLocaleResource("Admin.SmsTemplates.NoExisted");

            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.List");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Copy");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.List");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Copy");

            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Enabled");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.AccessKeyID");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.AccessKeySecret");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.SignName");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Product");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.Domain");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.RegionId");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.Settings.Fields.EndpointName");

            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.SystemName");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.TemplateCode");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.Subject");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.Body");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.IsActive");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.SendImmediately");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.SmsTemplate.Fields.DelayBeforeSend");

            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Priority");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.TemplateCode");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Subject");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.Body");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.ToMobileNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SendImmediately");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.ToName");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.TemplateParamJson");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.CreatedOn");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.DontSendBeforeDate");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SentTries");
            _localizationService.DeletePluginLocaleResource("Plugins.SMS.Aliyun.QueuedSms.Fields.SentOn");

            base.Uninstall();
        }


        #endregion

        #region Properties
        

        #endregion
    }
}
