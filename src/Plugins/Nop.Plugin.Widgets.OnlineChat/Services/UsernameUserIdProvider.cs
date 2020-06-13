using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Nop.Plugin.Widgets.Notification.Services
{
    public class UsernameUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            string id = connection.User?.FindFirst(ClaimTypes.Name)?.Value;
            if(id == null)
                id = connection.User?.FindFirst(ClaimTypes.Email)?.Value;

            return id;
        }
    }
}
