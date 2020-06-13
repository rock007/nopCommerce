﻿using Nop.Core.Events;
using Nop.Plugin.SMS.Aliyun.Domain;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.SMS.Aliyun.Infrastructure.Cache
{
    /// <summary>
    /// Event consumer of the "Fixed or by weight" shipping plugin (used for removing unused settings)
    /// </summary>
    public partial class QueuedSmsEventConsumer : IConsumer<EntityDeletedEvent<SmsTemplate>>
    {
        #region Fields
        
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public QueuedSmsEventConsumer(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shipping method deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(EntityDeletedEvent<SmsTemplate> eventMessage)
        {
            //var shippingMethod = eventMessage?.Entity;
            //if (shippingMethod == null)
            //    return;

            ////delete saved fixed rate if exists
            //var setting = _settingService.GetSetting(string.Format(AliyunSmsDefaults.FixedRateSettingsKey, shippingMethod.Id));
            //if (setting != null)
            //    _settingService.DeleteSetting(setting);
        }

        #endregion
    }
}