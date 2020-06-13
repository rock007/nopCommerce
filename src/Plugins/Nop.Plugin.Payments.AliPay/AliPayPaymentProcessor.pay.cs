using Essensoft.AspNetCore.Payment.Alipay.Domain;
using Essensoft.AspNetCore.Payment.Alipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Payments.AliPay
{
    /// <summary>
    /// 支付宝支付，支持当面付、手机网站支付、电脑网站支付,
    ///  
    /// </summary>
    public partial class AliPayPaymentProcessor
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
            string redirectUrl = "";
            // SDK 文档地址
            // https://docs.open.alipay.com/399/106844/
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var customValues = _paymentService.DeserializeCustomValues(postProcessPaymentRequest.Order);
            if (customValues != null && customValues.ContainsKey("PaymentMethod"))
            {
                // 1、当面付-扫码支付
                // 收银员通过收银台或商户后台调用支付宝接口，现场生成支付二维码后，展示给用户，由用户扫描二维码完成订单支付。
                // 流程说明：https://docs.open.alipay.com/194/105072
                // API接口：https://docs.open.alipay.com/api_1/alipay.trade.precreate
                if (customValues.GetValueOrDefault("PaymentMethod")?.ToString() == "alipay.trade.precreate")
                {
                    // 当面付-扫码支付 直接跳转到扫码页面
                    _httpContextAccessor.HttpContext.Response.Redirect(urlHelper.RouteUrl(AliPayDefaults.ScanCodeRouteName,
                        new { ordercode = postProcessPaymentRequest.Order.OrderGuid.ToString("N") }));
                }
                // 2、当面付-二维码/条码/声波支付
                // 收银员使用扫码设备读取用户手机支付宝“付款码”/声波获取设备（如麦克风）读取用户手机支付宝的声波信息后，
                // 将二维码或条码信息/声波信息通过本接口上送至支付宝发起支付。
                // 流程说明：https://docs.open.alipay.com/194/105072
                // API接口：https://docs.open.alipay.com/api_1/alipay.trade.pay
                if (customValues.GetValueOrDefault("PaymentMethod")?.ToString() == "alipay.trade.pay")
                {
                    // 使用扫描枪扫顾客支付宝App的付款码，直接扣款了
                    var model = new AlipayTradePrecreateModel
                    {
                        Body = GetBody(postProcessPaymentRequest.Order),
                        OutTradeNo = postProcessPaymentRequest.Order.OrderGuid.ToString("N"),
                        TotalAmount = postProcessPaymentRequest.Order.OrderTotal.ToString("#0.00"),
                        Subject = GetBody(postProcessPaymentRequest.Order),
                    };
                    var request = new AlipayTradePrecreateRequest();
                    request.SetBizModel(model);
                    request.SetNotifyUrl(_webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                        && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url)) 
                        + urlHelper.RouteUrl(AliPayDefaults.PrecreateNotifyRouteName));
                    var response = _payClient.ExecuteAsync(request);
                    // var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);
                    postProcessPaymentRequest.Order.CaptureTransactionResult = response.Result.Body;
                    return;
                }
            }
            string form = "";
            // 3、手机网站支付
            // 商家网页会跳转到支付宝中完成支付，支付完后跳回到商家网页内，最后展示支付结果。
            // 若无法唤起支付宝客户端(例如手机未安装支付宝)，则在一定的时间后会自动进入网页支付流程。
            if (_userAgentHelper.IsMobileDevice())
            {
                // 流程说明：https://docs.open.alipay.com/203/105288
                // API接口：https://docs.open.alipay.com/api_1/alipay.trade.wap.pay
                var model = new AlipayTradeWapPayModel
                {
                    Body = GetBody(postProcessPaymentRequest.Order),
                    Subject = GetBody(postProcessPaymentRequest.Order),
                    TotalAmount = postProcessPaymentRequest.Order.OrderTotal.ToString("#0.00"),
                    OutTradeNo = postProcessPaymentRequest.Order.OrderGuid.ToString("N"),
                    ProductCode = postProcessPaymentRequest.Order.OrderGuid.ToString()
                };
                var request = new AlipayTradeWapPayRequest();
                request.SetBizModel(model);
                request.SetNotifyUrl(_webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                        && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url))
                        + urlHelper.RouteUrl(AliPayDefaults.WapPayNotifyRouteName));
                request.SetReturnUrl(_webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                        && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url))
                        + urlHelper.RouteUrl(AliPayDefaults.ReturnRouteName));

                var response = _payClient.PageExecuteAsync(request);
                //var response = _payClient.PageExecuteAsync(request, _optionsAccessor.Value);
                //postProcessPaymentRequest.Order.CaptureTransactionResult = response.Result.Body;

                var ackResult = response.Result;

                form = ackResult.Body;
                postProcessPaymentRequest.Order.CaptureTransactionResult = form;
                //redirectUrl = ackResult.MwebUrl;
                //if (string.IsNullOrEmpty(redirectUrl))
                //{
                //    _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, ackResult.Msg, ackResult.Body);
                //}
            }
            else
            {
                // 4、电脑网站支付
                // 流程说明：https://docs.open.alipay.com/270/105898
                // API接口：https://docs.open.alipay.com/270/alipay.trade.page.pay
                var model = new AlipayTradePagePayModel
                {
                    Body = GetBody(postProcessPaymentRequest.Order),
                    Subject = GetBody(postProcessPaymentRequest.Order),
                    TotalAmount = postProcessPaymentRequest.Order.OrderTotal.ToString("#0.00"),
                    OutTradeNo = postProcessPaymentRequest.Order.OrderGuid.ToString("N"),
                    ProductCode = AliPayDefaults.ProductCode
                };
                var request = new AlipayTradePagePayRequest();
                request.SetBizModel(model);
                request.SetNotifyUrl(_webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                        && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url)).TrimEnd('/')
                        + urlHelper.RouteUrl(AliPayDefaults.PagePayNotifyRouteName));
                request.SetReturnUrl(_webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                        && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url)).TrimEnd('/')
                        + urlHelper.RouteUrl(AliPayDefaults.ReturnRouteName));
                // var response = await _client.PageExecuteAsync(req, _optionsAccessor.Value);
                //var response = _payClient.PageExecuteAsync(request);
                var response = _payClient.PageExecuteAsync(request, null, "GET");
                form = response.Result.Body;
                postProcessPaymentRequest.Order.CaptureTransactionResult = form;
                redirectUrl = form;
            }
            //return Content(response.Body, "text/html", Encoding.UTF8);

            //post
            //var httpContext = _httpContextAccessor.HttpContext;
            //var httpResponse = httpContext.Response;
            ////httpResponse.Clear();
            //var data = Encoding.UTF8.GetBytes(form);
            //httpResponse.ContentType = "text/html; charset=utf-8";
            //httpResponse.StatusCode = 200;
            //httpResponse.ContentLength = data.Length;
            //httpResponse.Body.Write(data, 0, data.Length);
            //httpResponse.Body.Flush();

            //store a value indicating whether POST has been done
            //_webHelper.IsPostBeingDone = true;

            // get
            if (string.IsNullOrEmpty(redirectUrl))
            {
                return;
            }
            _httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
        }

        /// <summary>
        /// Refunds a payment
        /// https://docs.open.alipay.com/api_1/alipay.trade.refund
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
            var order = refundPaymentRequest.Order;
            var model = new AlipayTradeRefundModel
            {
                OutTradeNo = order.OrderGuid.ToString(),
                TradeNo = order.CaptureTransactionResult,
                RefundAmount = order.OrderTotal.ToString("#0.00"),
                OutRequestNo = order.OrderGuid.ToString(),
                RefundReason = "退款原因说明"
            };

            var request = new AlipayTradeRefundRequest();
            request.SetBizModel(model);
            var response = _payClient.ExecuteAsync(request);
            var result = response.Result;

            if(result.Code != "10000")
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
            /**!!!
            int count = order.OrderItems.Count;

            foreach (Core.Domain.Orders.OrderItem item in order.OrderItems.OrderBy(oi => oi.Id))
            {
                body = body + item.Product.Name;
                if (p < count - 1)
                {
                    body = body + "|";
                }
                p++;
            }
            ***/
            if (body.Length > length)
            {
                body = body.Substring(0, length);
            }
            return body.Trim().Replace('&', '_').Replace('=', '_').Replace('-', '_');
        }


        #endregion
    }
}
