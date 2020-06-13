using Essensoft.AspNetCore.Payment.Alipay;
using Essensoft.AspNetCore.Payment.Alipay.Parser;
using Essensoft.AspNetCore.Payment.Alipay.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nop.Plugin.Payments.AliPay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Essensoft.AspNetCore.Payment.Alipay
{
    /// <summary>
    /// 支付宝支付，支持当面付、手机网站支付、电脑网站支付,
    /// 有问题请联系QQ，954219492@qq.com，QQ群：169366609
    /// </summary>
    public class DefaultAliPayNotifyClient : IAlipayNotifyClient
    {
        private readonly AliPayPaymentSettings _aliPayPaymentSettings;
        private readonly ILogger _logger;
        //private readonly IOptionsSnapshot<AlipayOptions> _optionsSnapshotAccessor;

        #region DefaultAliPayNotifyClient Constructors

        public DefaultAliPayNotifyClient(
            ILogger<DefaultAliPayNotifyClient> logger,
            //IOptionsSnapshot<AlipayOptions> optionsAccessor)
            AliPayPaymentSettings aliPayPaymentSettings)
        {
            _logger = logger;
            //_optionsSnapshotAccessor = optionsAccessor;
            _aliPayPaymentSettings = aliPayPaymentSettings;
        }

        #endregion

        #region IAlipayNotifyClient Members

        public async Task<T> ExecuteAsync<T>(HttpRequest request) where T : AlipayNotify
        {
            return await ExecuteAsync<T>(request, null);
        }

        public async Task<T> ExecuteAsync<T>(HttpRequest request, string optionsName) where T : AlipayNotify
        {
            //var options = _optionsSnapshotAccessor.Get(optionsName);
            var parameters = await GetParametersAsync(request);
            var query = AlipayUtility.BuildQuery(parameters);
            _logger.Log(_aliPayPaymentSettings.LogLevel, "Request:{query}", query);

            var parser = new AlipayDictionaryParser<T>();
            var rsp = parser.Parse(parameters);
            CheckNotifySign(parameters, _aliPayPaymentSettings);
            return rsp;
        }

        #endregion

        #region Common Method

        private async Task<SortedDictionary<string, string>> GetParametersAsync(HttpRequest request)
        {
            var parameters = new SortedDictionary<string, string>();
            if (request.Method == "POST")
            {
                var form = await request.ReadFormAsync();
                foreach (var iter in form)
                {
                    parameters.Add(iter.Key, iter.Value);
                }
            }
            else
            {
                foreach (var iter in request.Query)
                {
                    parameters.Add(iter.Key, iter.Value);
                }
            }
            return parameters;
        }

        private void CheckNotifySign(IDictionary<string, string> dictionary, AliPayPaymentSettings options)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                throw new AlipayException("sign check fail: dictionary is Empty!");
            }

            if (!dictionary.TryGetValue("sign", out var sign))
            {
                throw new AlipayException("sign check fail: sign is Empty!");
            }

            var prestr = GetSignContent(dictionary);
            if (!AlipaySignature.RSACheckContent(prestr, sign, options.PublicRSAParameters, options.SignType))
            {
                throw new AlipayException("sign check fail: check Sign Data Fail!");
            }
        }

        private string GetSignContent(IDictionary<string, string> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            var sb = new StringBuilder();
            foreach (var iter in dictionary)
            {
                if (!string.IsNullOrEmpty(iter.Value) && iter.Key != "sign" && iter.Key != "sign_type")
                {
                    sb.Append(iter.Key).Append("=").Append(iter.Value).Append("&");
                }
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        #endregion
    }
}
