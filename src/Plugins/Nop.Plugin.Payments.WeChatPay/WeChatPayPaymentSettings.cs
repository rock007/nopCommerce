using Nop.Core.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;

namespace Nop.Plugin.Payments.WeChatPay
{
    /// <summary>
    /// 
    /// </summary>
    public partial class WeChatPayPaymentSettings : ISettings
    {
        //internal ICipherParameters PublicKey;

        /// <summary>
        /// 绑定支付的APPID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置）
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 商户支付密钥
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// RSA公钥
        /// </summary>
        public string RsaPublicKey { get; set; }


        // https://pay.weixin.qq.com/wiki/doc/api/tools/mch_pay.php?chapter=4_3

        /// <summary>
        /// 证书文件路径
        /// </summary>
        public string SslCertificationPath { get; set; }
        /// <summary>
        /// 证书文件密码
        /// </summary>
        public string SslCertificationPassword { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
    }
}
