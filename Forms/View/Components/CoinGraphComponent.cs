using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Pages;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class CoinGraphComponent : ContentView
    {
        private readonly HybridWebView _webView;

        public CoinGraphComponent(INavigation navigation)
        {
            _webView = new HybridWebView("Html/pieChart.html") { LoadFinished = UpdateView };

            _webView.RegisterCallback("selectedCallback", id =>
            {
                var element = AccountStorage.Instance.AllElements.Find(e => e.Id == Convert.ToInt32(id));
                if (element != null) Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new AccountView(element)));
            });

            Content = _webView;
            HeightRequest = 500;

            UpdateView();

            Messaging.Loading.SubscribeFinished(this, UpdateView);
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeValueChanged(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeValueChanged(this, UpdateView);

            Messaging.Progress.SubscribeToComplete(this, UpdateView);
        }

        private void UpdateView()
        {

            var items = AccountStorage.AccountsGroupedByCurrency
                        .Select(e => new Data(e, ApplicationSettings.BaseCurrency)).Where(d => d.value > 0)
                        .OrderByDescending(d => d.value).ToArray();

            Device.BeginInvokeOnMainThread(() => _webView.CallJsFunction("showChart", items, new[] { I18N.OneAccount, I18N.Accounts }, new[] { I18N.OneCurrency, I18N.Currencies }, I18N.Further, I18N.NoDataToDisplay, ApplicationSettings.BaseCurrency.Code, ApplicationSettings.RoundMoney, CultureInfo.CurrentCulture.ToString()));
        }

        [DataContract]
        [JsonObject]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class Data
        {
            [DataMember]
            public readonly decimal value;

            [DataMember]
            public readonly string label;

            [DataMember]
            public readonly string money;

            [DataMember]
            public readonly string reference;

            [DataMember]
            public readonly string name;

            [DataMember]
            public readonly AccountData[] accounts;

            public Data(IGrouping<Currency, Account> group, Currency referenceCurrency)
            {
                var rate = new ExchangeRate(group.Key, referenceCurrency);
                rate = ExchangeRateHelper.GetRate(rate) ?? rate;

                var totalMoney = new Money(group.Sum(a => a.IsEnabled ? a.Money.Amount : 0), group.Key);

                label = group.Key.Code;
                name = group.Key.Name;
                value = totalMoney.Amount * rate.Rate ?? 0;
                money = totalMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                reference = new Money(value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                accounts = group.Select(a => new AccountData(a, rate, referenceCurrency)).Where(d => d.value > 0).OrderByDescending(d => d.value).ToArray();
            }
        }

        [DataContract]
        [JsonObject]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class AccountData
        {
            [DataMember]
            public readonly decimal value;

            [DataMember]
            public readonly string label;

            [DataMember]
            public readonly string money;

            [DataMember]
            public readonly string reference;

            [DataMember]
            public readonly int id;

            public AccountData(Account account, ExchangeRate rate, Currency referenceCurrency)
            {
                value = account.IsEnabled ? account.Money.Amount * rate.Rate ?? 0 : 0;
                label = account.Name;
                money = (account.IsEnabled ? account.Money : new Money(0, account.Money.Currency)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                reference = new Money(value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                id = account.Id;
            }
        }
    }
}
