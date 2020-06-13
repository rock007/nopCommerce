using Essensoft.AspNetCore.Payment.Alipay;
using Essensoft.AspNetCore.Payment.Alipay.Domain;
using Essensoft.AspNetCore.Payment.Alipay.Notify;
using Essensoft.AspNetCore.Payment.Alipay.Request;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.AliPay.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.AliPay.Controllers
{
    public partial class PaymentAliPayController : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;

        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        private readonly IAlipayClient _payClient;
        private readonly IAlipayNotifyClient _notifyClient;
        private readonly AliPayPaymentSettings _aliPayPaymentSettings;
        private readonly IOptions<AlipayOptions> _optionsAccessor;

        #endregion

        #region Ctor

        public PaymentAliPayController(
            IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            IAlipayClient payClient,
            IAlipayNotifyClient notifyClient,
            ShoppingCartSettings shoppingCartSettings,
            AliPayPaymentSettings aliPayPaymentSettings,
            IOptions<AlipayOptions> optionsAccessor)
        {
            this._genericAttributeService = genericAttributeService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._paymentService = paymentService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._logger = logger;
            this._notificationService = notificationService;
            this._settingService = settingService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._workContext = workContext;

            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;

            _payClient = payClient;
            _notifyClient = notifyClient;
            _aliPayPaymentSettings = aliPayPaymentSettings;
            _optionsAccessor = optionsAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 选择微信支付后，重定向到扫码页面
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public IActionResult AliPayScanCode(string ordercode)
        {
            var model = new AliPayScanCodeModel();
            model.OrderCode = ordercode;
            //this.RouteData
            return View("~/Plugins/Payments.AliPay/Views/AliPayScanCode.cshtml", model);
        }

        /// <summary>
        /// 获取到微信支付二维码
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public async Task<IActionResult> MakeQRCode(string ordercode)
        {
            if(String.IsNullOrEmpty(ordercode) || ordercode.Length != 32)
            {
                return File("/content/default-image.gif", "image/gif");
            }
            var order = _orderService.GetOrderByGuid(new Guid( ordercode));
            if (order == null)
            {
                return File("/content/default-image.gif", "image/gif");
            }
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var model = new AlipayTradePrecreateModel
            {
                Body = GetBody(order),
                OutTradeNo = order.OrderGuid.ToString("N"),
                TotalAmount = order.OrderTotal.ToString("#0.00"),
                Subject = GetBody(order),
            };
            var request = new AlipayTradePrecreateRequest();
            request.SetBizModel(model);
            request.SetNotifyUrl(urlHelper.RouteUrl(AliPayDefaults.PrecreateNotifyRouteName));
            var response = await _payClient.ExecuteAsync(request);
            
            // response.CodeUrl 给前端生成二维码
            ViewData["qrcode"] = response.QrCode; //  "qr_code": "https://qr.alipay.com/bavh4wjlxf12tper3a"
            ViewData["response"] = response.Body;

            //将字符串生成二维码图片
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(response.QrCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //Bitmap image = qrCodeEncoder.Encode(result.CodeUrl, Encoding.Default);

            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            return File(ms.GetBuffer(), "image/jpeg");   
        }

        [HttpPost]
        public async Task<IActionResult> WapPayNotify()
        {
            try
            {
                var notify = await _notifyClient.ExecuteAsync<AlipayTradeWapPayNotify>(Request);
                if ("TRADE_SUCCESS" == notify.TradeStatus)
                {
                    var order = _orderService.GetOrderByGuid(new Guid(notify.OutTradeNo));
                    if (order == null)
                    {
                        return AlipayNotifyResult.Failure;
                    }
                    //if (notify.ReceiptAmount != Convert.ToInt32(order.OrderTotal * 100).ToString())
                    if (Convert.ToDecimal(notify.ReceiptAmount) != order.OrderTotal) // Convert.ToInt32(order.OrderTotal * 100).ToString()
                    {
                        return AlipayNotifyResult.Failure;
                    }
                    if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionCode = notify.TradeNo;
                        order.AuthorizationTransactionId = notify.TradeNo;
                        order.SubscriptionTransactionId = notify.TradeNo;
                        order.CaptureTransactionId = notify.TradeNo;
                        order.CaptureTransactionResult = notify.TradeNo;
                        _orderProcessingService.MarkOrderAsPaid(order);
                    }
                    return AlipayNotifyResult.Success;
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
        }
        [HttpPost]
        public async Task<IActionResult> PagePayNotify()
        {
            try
            {
                var notify = await _notifyClient.ExecuteAsync<AlipayTradePagePayNotify>(Request);
                if ("TRADE_SUCCESS" == notify.TradeStatus)
                {
                    var order = _orderService.GetOrderByGuid(new Guid(notify.OutTradeNo));
                    if (order == null)
                    {
                        return AlipayNotifyResult.Failure;
                    }
                    if (Convert.ToDecimal(notify.ReceiptAmount) != order.OrderTotal) // Convert.ToInt32(order.OrderTotal * 100).ToString()
                    {
                        return AlipayNotifyResult.Failure;
                    }
                    if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionCode = notify.TradeNo;
                        order.AuthorizationTransactionId = notify.TradeNo;
                        order.SubscriptionTransactionId = notify.TradeNo;
                        order.CaptureTransactionId = notify.TradeNo;
                        order.CaptureTransactionResult = notify.NotifyTime;
                        _orderProcessingService.MarkOrderAsPaid(order);
                    }
                    return AlipayNotifyResult.Success;
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
        }
        [HttpPost]
        public async Task<IActionResult> PrecreateNotify()
        {
            try
            {
                var notify = await _notifyClient.ExecuteAsync<AlipayTradePrecreateNotify>(Request);
                if ("TRADE_SUCCESS" == notify.TradeStatus)
                {
                    //Console.WriteLine("OutTradeNo: " + notify.OutTradeNo);
                    //return MarkOrderAsPaid(notify);
                    var order = _orderService.GetOrderByGuid(new Guid(notify.OutTradeNo));
                    if (order == null)
                    {
                        return AlipayNotifyResult.Failure;
                    }
                    if (Convert.ToDecimal(notify.ReceiptAmount) != order.OrderTotal) // Convert.ToInt32(order.OrderTotal * 100).ToString()
                    {
                        return AlipayNotifyResult.Failure;
                    }
                    if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionCode = notify.TradeNo;
                        order.AuthorizationTransactionId = notify.TradeNo;
                        order.SubscriptionTransactionId = notify.TradeNo;
                        order.CaptureTransactionId = notify.TradeNo;
                        order.CaptureTransactionResult = notify.NotifyTime;
                        _orderProcessingService.MarkOrderAsPaid(order);
                    }
                    return AlipayNotifyResult.Success;
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
        }


        // 对于PC网站支付的交易，在用户支付完成之后，支付宝会根据API中商户传入的return_url参数，
        // 通过GET请求的形式将部分支付结果参数通知到商户系统。
        public ActionResult Return(string orderId)
        {
            if (String.IsNullOrEmpty(_aliPayPaymentSettings.ReturnRouteName))
            {
                return RedirectToRoute("CustomerOrders");
            }
            else
            {
                // https://github.com/essensoft/payment/blob/dev/samples/WebApplicationSample/Controllers/AlipayController.cs
                //var notify = _notifyClient.ExecuteAsync<AlipayTradePagePayReturn>(Request, _optionsAccessor.Value);
                //if (string.IsNullOrEmpty(orderId))
                //{
                //}
                //else
                //{

                //}
                return RedirectToRoute(_aliPayPaymentSettings.ReturnRouteName, new { orderId = orderId });
            }
        }

        //private IActionResult MarkOrderAsPaid<T>(T notify)
        //{
        //    var order = _orderService.GetOrderByGuid(new Guid(notify.OutTradeNo));
        //    if (order == null)
        //    {
        //        return AlipayNotifyResult.Failure;
        //    }
        //    if (notify.ReceiptAmount != Convert.ToInt32(order.OrderTotal * 100).ToString())
        //    {
        //        return AlipayNotifyResult.Failure;
        //    }
        //    if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
        //    {
        //        order.AuthorizationTransactionCode = notify.TradeNo;
        //        order.AuthorizationTransactionId = notify.TradeNo;
        //        order.SubscriptionTransactionId = notify.TradeNo;
        //        order.CaptureTransactionId = notify.TradeNo;
        //        order.CaptureTransactionResult = notify.NotifyTime;
        //        _orderProcessingService.MarkOrderAsPaid(order);
        //    }
        //    return AlipayNotifyResult.Success;
        //}

        #endregion

        #region

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
            return body.Trim().Replace('&', '_').Replace('=', '_');
        }

        public string GetBody(IList<Order> orders, int length = 224)
        {
            string body = "";

            if (orders.Count > 0)
            {
                length = length / orders.Count;
            }

            /**
            foreach (var order in orders)
            {
                int count = order.OrderItems.Count;
                if (count == 0)
                    continue;
                int p = 0;
                string tmp = "";
                foreach (Core.Domain.Orders.OrderItem item in order.OrderItems)
                {
                    tmp = tmp + item.Product.Name;
                    if (p < count - 1)
                    {
                        tmp = tmp + "|";
                    }
                    p++;
                }
                if (tmp.Length > length)
                {
                    tmp = tmp.Substring(0, length);
                }

                body += tmp;
            }
            ****/
            return body;
        }

        #endregion
    }
}