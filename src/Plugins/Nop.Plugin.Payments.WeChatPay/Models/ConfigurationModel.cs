using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.WeChatPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        /// <summary>
        /// 绑定支付的APPID（必须配置） 公众账号ID String(32)	appid
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.AppId")]
        public string AppId { get; set; }
        public bool AppId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.AppSecret")]
        public string AppSecret { get; set; }
        public bool AppSecret_OverrideForStore { get; set; }
        /// <summary>
        /// 商户号（必须配置）mch_id	String(32)	微信支付分配的商户号
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.Key")]
        public string Key { get; set; }
        public bool Key_OverrideForStore { get; set; }
        /// <summary>
        /// 证书存放路径
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.SslCertificationPath")]
        public string SslCertificationPath { get; set; }
        public bool SslCertificationPath_OverrideForStore { get; set; }
        /// <summary>
        /// 证书的密码
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.SslCertificationPassword")]
        public string SslCertificationPassword { get; set; }
        public bool SslCertificationPassword_OverrideForStore { get; set; }

        /// <summary>
        /// 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.ReportLevel")]
        public LogLevel ReportLevel { get; set; }
        public bool ReportLevel_OverrideForStore { get; set; }
        /// <summary>
        /// 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.WeChatPay.LogLevel")]
        public LogLevel LogLevel { get; set; }
        public bool LogLevel_OverrideForStore { get; set; }
        //[NopResourceDisplayName("Plugins.Payments.WeChatPay.SellerEmail")]
        //public string SellerEmail { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.WeChatPay.Key")]
        //public string Key { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.WeChatPay.Partner")]
        //public string Partner { get; set; }

        [NopResourceDisplayName("Plugins.Payments.WeChatPay.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.WeChatPay.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}