using Essensoft.AspNetCore.Payment.WeChatPay.Request;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Nop.Plugin.Payments.WeChatPay
{
    public partial class WeChatPayPaymentProcessor
    {
        #region Methods

        /// <summary>
        /// Process a payment
        /// 订单生成之前处理付款信息，准备好付款参数后，此方法主要生成付款授权码、查询付款额度等等，
        /// 用于付款之前的参数收集，注意此时订单还没有生成，更没有去付款，只是准备订单信息、验证是否能付款。
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (postProcessPaymentRequest == null || postProcessPaymentRequest.Order == null)
            {
                return;
            }
            // 1、先判断是否为刷卡支付
            var customValues = _paymentService.DeserializeCustomValues(postProcessPaymentRequest.Order);
            if(customValues!= null && customValues.ContainsKey("PaymentMethod") 
                && customValues.GetValueOrDefault("PaymentMethod")?.ToString() == "WeChatMicroPay")
            {
                var request = new WeChatPayMicroPayRequest
                {
                    Body = GetBody(postProcessPaymentRequest.Order),
                    OutTradeNo = postProcessPaymentRequest.Order.OrderGuid.ToString("N"),
                    TotalFee = Convert.ToInt32(postProcessPaymentRequest.Order.OrderTotal * 100),
                    SpbillCreateIp = _webHelper.GetCurrentIpAddress(),
                    AuthCode = customValues.GetValueOrDefault("AuthCode")?.ToString()
                };
                //var response = await _client.ExecuteAsync(request);
                // var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);
                var response = _payClient.ExecuteAsync(request);
                postProcessPaymentRequest.Order.CaptureTransactionResult = response.Result.Body;
                // 如果是刷卡支付，则支付完成后直接返回，等待通知
                // https://pay.weixin.qq.com/wiki/doc/api/micropay.php?chapter=5_4
                // 商户收银后台对得到的支付结果进行签名验证和处理，再将支付结果返回给门店收银台。
                return;
            }

            string redirectUrl = "";
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            if(_userAgentHelper.IsMobileDevice())
            {
                // 2、 如果是H5支付，则需要重定向到微信网站进行支付
                // 重定向的URL地址：https://api.mch.weixin.qq.com/pay/unifiedorder
                var request = new WeChatPayUnifiedOrderRequest
                {
                    Body =  GetBody(postProcessPaymentRequest.Order),
                    OutTradeNo = postProcessPaymentRequest.Order.OrderGuid.ToString("N"),
                    TotalFee = Convert.ToInt32(postProcessPaymentRequest.Order.OrderTotal * 100),
                    SpbillCreateIp = _webHelper.GetCurrentIpAddress(),
                    //NotifyUrl = urlHelper.RouteUrl(WeChatPayDefaults.NotifyRouteName),
                    NotifyUrl = _webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                    && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url))
                    + WeChatPayDefaults.NotifyUrl,
                    TradeType = WeChatTradeTypeDefaults.MobileWeb
                };
                //var response = await _client.ExecuteAsync(request);
                var response = _payClient.ExecuteAsync(request);
                var ackResult = response.Result;
                redirectUrl = ackResult.MwebUrl;
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    //_logger.Error(response.Result.ReturnMsg);
                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, ackResult.ReturnMsg, ackResult.Body);
                }
            }
            else
            {
                // 3、如果是扫码支付，则进行网站内部的重定向，重定向到 扫码页面
                redirectUrl = urlHelper.RouteUrl(WeChatPayDefaults.ScanCodeRouteName,
                new { ordercode = postProcessPaymentRequest.Order.OrderGuid.ToString("N") });
            }
            if(string.IsNullOrEmpty(redirectUrl))
            {
                return;
            }
            _httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
        }

        

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            //return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
            if (refundPaymentRequest == null || refundPaymentRequest.Order == null)
            {
                return new RefundPaymentResult { Errors = new[] { "请选定要退款的订单" } };
            }
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var order = refundPaymentRequest.Order;
            var request = new WeChatPayRefundRequest
            {
                OutRefundNo = order.OrderGuid.ToString(), // 退款单号，临时用订单号代替, guid有横线
                TransactionId = order.CaptureTransactionResult,
                OutTradeNo = order.OrderGuid.ToString("N"),
                TotalFee = Convert.ToInt32(order.OrderTotal * 100),
                RefundFee = Convert.ToInt32(order.OrderTotal * 100),
                RefundDesc = "退款：" + GetBody(order),
                NotifyUrl = _webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                    && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url))
                    + WeChatPayDefaults.RefundNotifyUrl
                //NotifyUrl = urlHelper.RouteUrl(WeChatPayDefaults.NotifyRouteName)
            };
            _logger.Information(request.NotifyUrl);
            var response = _payClient.ExecuteAsync(request, "wechatpayCertificateName");
            var result = response.Result;

            if(result.ReturnCode == "FAIL")
            {
                return new RefundPaymentResult { Errors = new[] { "退款失败", result.Body } };
            }
            return new RefundPaymentResult { NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Refunded};

        }

        #endregion

        #region Helper

        public static string GetBody(Order order, int length = 63)
        {
            string body = "";
            int p = 0;
            /****
            int count = order.OrderItems.Count;

            foreach (OrderItem item in order.OrderItems.OrderBy(oi => oi.Id))
            {
                body = body + item.Product.Name;
                if (p < count - 1)
                {
                    body = body + "|";
                }
                p++;
            }
            ****/
            if (body.Length > length)
            {
                body = body.Substring(0, length);
            }
            return body.Trim().Replace('&', '_').Replace('=', '_');
        }


        #endregion
    }
}
