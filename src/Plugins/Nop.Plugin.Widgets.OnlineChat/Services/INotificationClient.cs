using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Notification.Services
{
    /// <summary>
    /// 客户端监听事件：服务器端向客户端发送消息，客户端接收服务器端的消息
    /// </summary>
    public interface INotificationClient
    {
        // 当客户端需要监听服务器端的方法调用时，默认情况下是采用 双方约定的一个字符串作为键值。

        /// <summary>
        /// 新会员注册通知
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveCustomerRegisteredMessage(string user, object message);
        /// <summary>
        /// 订单已撤销通知
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveOrderCancelledMessage(string user, object message);
        /// <summary>
        /// 前端网站下订单成功的通知
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveOrderPlacedMessage(string user, object message);
        /// <summary>
        /// 前端商城支付成功的通知
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveOrderPaidMessage(string user, object message);
        /// <summary>
        /// 订单已退款通知
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task ReceiveOrderRefundedMessage(string user, object message);
    }
}
