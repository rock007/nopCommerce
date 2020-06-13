using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Customers;
using System.Threading.Tasks;
using Nop.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Nop.Plugin.SMS.Aliyun.Services
{
    public partial class ShortMessageTokenProvider : IShortMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;

        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly MessageTemplatesSettings _templatesSettings;

        //private Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="languageService">Language service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="downloadService">Download service</param>
        /// <param name="orderService">Order service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="storeService">Store service</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="addressAttributeFormatter">Address attribute formatter</param>
        /// <param name="customerAttributeFormatter">Customer attribute formatter</param>
        /// <param name="urlHelperFactory">URL Helper factory</param>
        /// <param name="actionContextAccessor">Action context accessor</param>
        /// <param name="templatesSettings">Templates settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="storeInformationSettings">StoreInformation settings</param>
        public ShortMessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            ICustomerAttributeFormatter customerAttributeFormatter,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
           
            IStoreService storeService,
            IStoreContext storeContext,
            
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            MessageTemplatesSettings templatesSettings,
            
            IEventPublisher eventPublisher,
            StoreInformationSettings storeInformationSettings)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._customerAttributeFormatter = customerAttributeFormatter;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            this._workContext = workContext;
            
            this._urlHelperFactory = urlHelperFactory;
            this._actionContextAccessor = actionContextAccessor;
            this._storeService = storeService;
            this._storeContext = storeContext;

            this._templatesSettings = templatesSettings;
            
            this._eventPublisher = eventPublisher;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

        protected virtual IUrlHelper GetUrlHelper()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        //protected virtual string GetStoreUrl(int storeId = 0, bool removeTailingSlash = true)
        //{
        //    var store = _storeService.GetStoreById(storeId) ?? _storeContext.CurrentStore;

        //    if (store == null)
        //        throw new Exception("No store could be loaded");

        //    var url = store.Url;
        //    if (string.IsNullOrEmpty(url))
        //        throw new Exception("URL cannot be null");

        //    if (url.EndsWith("/"))
        //        url = url.Remove(url.Length - 1);

        //    return url;
        //}
        protected virtual string RouteUrl(int storeId = 0, string routeName = null, object routeValues = null)
        {
            //try to get a store by the passed identifier
            var store = _storeService.GetStoreById(storeId) ?? _storeContext.CurrentStore
                ?? throw new Exception("No store could be loaded");

            //ensure that the store URL is specified
            if (string.IsNullOrEmpty(store.Url))
                throw new Exception("URL cannot be null");

            //generate a URL with an absolute path
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = new PathString(urlHelper.RouteUrl(routeName, routeValues));

            //remove the application path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
            url.StartsWithSegments(pathBase, out url);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{store.Url.TrimEnd('/')}{url}"));
        }



        public virtual void AddStoreTokens(IList<Token> tokens, Store store)
        {
            tokens.Add(new Token("Store.Name", _localizationService.GetLocalized(store, x => x.Name)));
            //tokens.Add(new Token("Store.Name", store.GetLocalized(x => x.Name)));
            tokens.Add(new Token("Store.URL", store.Url, true));
            //tokens.Add(new Token("Store.Email", emailAccount.Email));
            tokens.Add(new Token("Store.CompanyName", store.CompanyName));
            tokens.Add(new Token("Store.CompanyAddress", store.CompanyAddress));
            tokens.Add(new Token("Store.CompanyPhoneNumber", store.CompanyPhoneNumber));
            tokens.Add(new Token("Store.CompanyVat", store.CompanyVat));

            tokens.Add(new Token("Facebook.URL", _storeInformationSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL", _storeInformationSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL", _storeInformationSettings.YoutubeLink));
            tokens.Add(new Token("GooglePlus.URL", _storeInformationSettings.GooglePlusLink));

            //event notification
            _eventPublisher.EntityTokensAdded(store, tokens);
        }

        public virtual void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", _customerService.GetCustomerFullName(customer)));
            tokens.Add(new Token("Customer.FirstName", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute)));
            tokens.Add(new Token("Customer.LastName", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute)));
            tokens.Add(new Token("Customer.VatNumber", _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute)));
            tokens.Add(new Token("Customer.VatNumberStatus", ((VatNumberStatus)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute)).ToString()));

            var customAttributesXml = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
            tokens.Add(new Token("Customer.CustomAttributes", _customerAttributeFormatter.FormatAttributes(customAttributesXml), true));

            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = RouteUrl(routeName: "PasswordRecoveryConfirm", routeValues: new { token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute), email = customer.Email });
            var accountActivationUrl = RouteUrl(routeName: "AccountActivation", routeValues: new { token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute), email = customer.Email });
            var emailRevalidationUrl = RouteUrl(routeName: "EmailRevalidation", routeValues: new { token = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute), email = customer.Email });
            var wishlistUrl = RouteUrl(routeName: "Wishlist", routeValues: new { customerGuid = customer.CustomerGuid });
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            tokens.Add(new Token("Customer.EmailRevalidationURL", emailRevalidationUrl, true));
            tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }
    }
}
