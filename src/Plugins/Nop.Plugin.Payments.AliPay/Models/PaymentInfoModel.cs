﻿namespace Nop.Plugin.Payments.AliPay.Models
{
    public partial class PaymentInfoModel
    {
        public PaymentInfoModel()
        {
            //IUserAgentHelper agent = (IUserAgentHelper)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IUserAgentHelper));
            //if (agent.IsMobileDevice())
        }

        public bool IsMobileDevice { get; set; }
    }
}
