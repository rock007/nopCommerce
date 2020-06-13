using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.AliPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        /// <summary>
        /// 绑定支付的APPID（必须配置） 公众账号ID String(32)	appid
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.AliPay.ApplicationId")]
        public string ApplicationId { get; set; }
        public bool ApplicationId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.RsaPublicKey")]
        public string RsaPublicKey { get; set; }
        public bool RsaPublicKey_OverrideForStore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.AliPay.RsaPrivateKey")]
        public string RsaPrivateKey { get; set; }
        public bool RsaPrivateKey_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.AliPay.EncyptKey")]
        public string EncyptKey { get; set; }
        public bool EncyptKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.ServerUrl")]
        public string ServerUrl { get; set; }
        public bool ServerUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.Version")]
        public string Version { get; set; }
        public bool Version_OverrideForStore { get; set; }




        [NopResourceDisplayName("Plugins.Payments.AliPay.Format")]
        public string Format { get; set; }
        public bool Format_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.SignType")]
        public string SignType { get; set; }
        public bool SignType_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.Charset")]
        public string Charset { get; set; }
        public bool Charset_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.EncyptType")]
        public string EncyptType { get; set; }
        public bool EncyptType_OverrideForStore { get; set; }


        ///// <summary>
        ///// 证书存放路径
        ///// </summary>
        //[NopResourceDisplayName("Plugins.Payments.AliPay.SslCertificationPath")]
        //public string SslCertificationPath { get; set; }
        ///// <summary>
        ///// 证书的私钥密码
        ///// </summary>
        //[NopResourceDisplayName("Plugins.Payments.AliPay.SslCertificationPassword")]
        //public string SslCertificationPassword { get; set; }

        /// <summary>
        /// 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.AliPay.ReportLevel")]
        public LogLevel ReportLevel { get; set; }
        public bool ReportLevel_OverrideForStore { get; set; }
        /// <summary>
        /// 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.AliPay.LogLevel")]
        public LogLevel LogLevel { get; set; }
        public bool LogLevel_OverrideForStore { get; set; }
        //[NopResourceDisplayName("Plugins.Payments.AliPay.SellerEmail")]
        //public string SellerEmail { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.AliPay.Key")]
        //public string Key { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.AliPay.Partner")]
        //public string Partner { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.AliPay.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}