using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.Notification.Services
{
    public class NotificationHelper
    {
        public static string GetBody(Order order, int length = 224)
        {
            string body = "";
            int p = 0;
            int count = order.OrderItems.Count;

            foreach (OrderItem item in order.OrderItems)
            {
                body = body + item.Product.Name;
                if (p < count - 1)
                {
                    body = body + "|";
                }
                p++;
            }
            if (body.Length > length)
            {
                body = body.Substring(0, length);
            }
            return body;
        }
    }
}
