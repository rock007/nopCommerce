using Microsoft.AspNetCore.SignalR;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.Notification.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Notification.Hubs
{
    public class NotificationHub : Hub<INotificationClient>
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public NotificationHub(ISettingService settingService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _settingService = settingService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        #endregion

        public override async Task OnConnectedAsync()
        {
            // 查看当前连接的用户是否属于管理员
            var identityName = Context.User.Identity.Name;
            var user = _customerService.GetCustomerByUsername(Context.User.Identity.Name);
            if(user == null)
                _customerService.GetCustomerByUsername(Context.User.Identity.Name);
            if(user != null && user.IsAdmin())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, NopCustomerDefaults.AdministratorsRoleName);
            }
            await base.OnConnectedAsync();

            //return Task.CompletedTask;
        }

        //public async Task SendMessage(string user, string message)
        //{
        //    // 
        //    await Clients.All.ReceiveOrderPaidMessage(user, message);
        //}

        //public Task SendMessageToCaller(string user, string message)
        //{
        //    return Clients.Caller.ReceiveOrderPlacedMessage(user, message);
        //}

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, NopCustomerDefaults.AdministratorsRoleName);
            await base.OnDisconnectedAsync(exception);

        }
        //public override async Task OnConnectedAsync()
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, "group1");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "group1");
        //    await base.OnDisconnectedAsync(exception);
        //}


    }
}
