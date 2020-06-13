using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.AliPay
{
    public static class RemotePostExtensions
    {
        public static void Post(this RemotePost post, string form)
        {
            //var httpContext =  _httpContextAccessor.HttpContext;
            //var response = httpContext.Response;
            //response.Clear();
            //var data = Encoding.UTF8.GetBytes(sb.ToString());
            //response.ContentType = "text/html; charset=utf-8";
            //response.ContentLength = data.Length;

            //response.Body.WriteAsync(data, 0, data.Length).Wait();

            ////store a value indicating whether POST has been done
            //_webHelper.IsPostBeingDone = true;
        }
    }
}
