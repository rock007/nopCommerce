using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.Notify;
using Essensoft.AspNetCore.Payment.WeChatPay.Request;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.WeChatPay.Models;
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

namespace Nop.Plugin.Payments.WeChatPay.Controllers
{
    public partial class PaymentWeChatPayController : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        private readonly IWeChatPayClient _payClient;
        private readonly IWeChatPayNotifyClient _notifyClient;
        private readonly WeChatPayPaymentSettings _weChatPayPaymentSettings;

        #endregion

        #region Ctor

        public PaymentWeChatPayController(
            IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWeChatPayClient payClient,
            IWeChatPayNotifyClient notifyClient,
            ShoppingCartSettings shoppingCartSettings,
            WeChatPayPaymentSettings weChatPayPaymentSettings)
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

            _payClient = payClient;
            _notifyClient = notifyClient;
            _weChatPayPaymentSettings = weChatPayPaymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 选择微信支付后，重定向到扫码页面
        /// </summary>
        /// <param name="ordercode"></param>
        /// <returns></returns>
        public IActionResult WeChatScanCode(string ordercode)
        {
            var model = new WeChatScanCodeModel();
            model.OrderCode = ordercode;
            //this.RouteData
            return View("~/Plugins/Payments.WeChatPay/Views/WeChatScanCode.cshtml", model);
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

            var request = new WeChatPayUnifiedOrderRequest
            {
                Body = GetBody(order),
                OutTradeNo = order.OrderGuid.ToString("N"),
                TotalFee = Convert.ToInt32(order.OrderTotal * 100),
                SpbillCreateIp = _webHelper.GetCurrentIpAddress(),
                NotifyUrl = _webHelper.GetStoreHost(_storeContext.CurrentStore.SslEnabled
                    && !String.IsNullOrWhiteSpace(_storeContext.CurrentStore.Url))
                    + "Plugins/PaymentWeChatPay/Notify",
                TradeType = WeChatPayDefaults.TradeType
            };
            var response = await _payClient.ExecuteAsync(request);
            // response.CodeUrl 给前端生成二维码
            ViewData["qrcode"] = response.CodeUrl;
            ViewData["response"] = response.Body;

            //将字符串生成二维码图片
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(response.CodeUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //Bitmap image = qrCodeEncoder.Encode(result.CodeUrl, Encoding.Default);

            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            return File(ms.GetBuffer(), "image/jpeg");   
        }

        [HttpPost]
        public async Task<IActionResult> Notify()
        {
            try
            {
                var notify = await _notifyClient.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request);
                if(notify == null)
                    return NoContent();
                _logger.Information(notify.Body);//记录一下回调通知日志，正式应用删除
                if (notify.ReturnCode == "SUCCESS")
                {
                    if (notify.ResultCode == "SUCCESS")
                    {
                        //Console.WriteLine("OutTradeNo: " + notify.OutTradeNo);
                        var order = _orderService.GetOrderByGuid(new Guid(notify.OutTradeNo));
                        if (order == null)
                        {
                            return WeChatPayNotifyResult.Failure;
                        }
                        if (notify.TotalFee != Convert.ToInt32(order.OrderTotal * 100).ToString())
                        {
                            return WeChatPayNotifyResult.Failure;
                        }
                        if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                        {
                            order.AuthorizationTransactionCode = notify.TransactionId;
                            order.AuthorizationTransactionId = notify.TransactionId;
                            order.SubscriptionTransactionId = notify.TransactionId;
                            order.CaptureTransactionId = notify.TransactionId;
                            order.CaptureTransactionResult = notify.TransactionId;
                            _orderProcessingService.MarkOrderAsPaid(order);
                        }

                        return WeChatPayNotifyResult.Success;
                    }
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
        }

        //[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [HttpPost()]
        [HttpGet()]
        public async Task<IActionResult> RefundNotify()
        {
            try
            {
                var notify = await _notifyClient.ExecuteAsync<WeChatPayRefundNotify>(Request);
                if (notify == null)
                    return NoContent();
                _logger.Information("退款信息：" + notify.Body);
                if (notify.ReturnCode == "SUCCESS")
                {
                    if (notify.RefundStatus == "SUCCESS")
                    {
                        Console.WriteLine("OutTradeNo: " + notify.OutTradeNo);
                        return WeChatPayNotifyResult.Success;
                    }
                }
                return NoContent();
            }
            catch
            {
                return NoContent();
            }
        }

        #endregion

        #region

        public static string GetBody(Order order, int length = 63)
        {
            string body = "";
            int p = 0;
            /**!!!
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

        public string GetBody(IList<Order> orders, int length = 224)
        {
            string body = "";

            if (orders.Count > 0)
            {
                length = length / orders.Count;
            }
            /****!!!
            foreach (var order in orders)
            {
                int count = order.OrderItems.Count;
                if (count == 0)
                    continue;
                int p = 0;
                string tmp = "";
                foreach (OrderItem item in order.OrderItems)
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
            *****/
            return body;
        }

        #endregion
    }
}