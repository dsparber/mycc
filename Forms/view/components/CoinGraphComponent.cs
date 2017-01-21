using System;
using System.Linq;
using MyCryptos.Forms.Messages;
using MyCC.Forms.Resources;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using MyCryptos.Forms.view.pages;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.ExchangeRate.Helpers;
using MyCC.Core.ExchangeRate.Model;
using MyCC.Core.Settings;

namespace MyCryptos.Forms.view.components
{
    public class CoinGraphComponent : ContentView
    {
        private readonly HybridWebView webView;
        private bool appeared;

        public CoinGraphComponent(INavigation navigation)
        {
            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>();

            webView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                MinimumHeightRequest = 500
            };
            webView.RegisterCallback("selectedCallback", id =>
            {
                var element = AccountStorage.Instance.AllElements.Find(e => e.Id == Convert.ToInt32(id));

                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new AccountDetailView(element)));
            });

            Content = webView;
            HeightRequest = 500;

            UpdateView();

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);

            Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
        }

        public void OnAppearing()
        {
            if (!appeared)
            {
                appeared = true;
                webView.LoadFromContent("Html/pieChart.html");
                Task.Delay(500).ContinueWith(t => UpdateView());
                UpdateView();
            }
        }

        public void UpdateView()
        {
            try
            {
                var items = AccountStorage.AccountsGroupedByCurrency.Select(e => new Data(e, ApplicationSettings.BaseCurrency)).Where(d => d.value > 0).OrderByDescending(d => d.value).ToArray();
                webView.CallJsFunction("showChart", items, new string[] { I18N.OneAccount, I18N.Accounts }, new string[] { I18N.OneCurrency, I18N.Currencies }, I18N.Further, I18N.NoDataToDisplay);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        [DataContract]
        [JsonObject]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
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

                var totalMoney = new Money(group.Sum(a => a.Money.Amount), group.Key);

                label = group.Key.Code;
                name = group.Key.Name;
                value = totalMoney.Amount * rate.RateNotNull;
                money = totalMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                reference = new Money(value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                accounts = group.Where(a => a.Money.Amount > 0).Select(a => new AccountData(a, rate, referenceCurrency)).OrderByDescending(d => d.value).ToArray();
            }
        }

        [DataContract]
        [JsonObject]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
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
                value = account.Money.Amount * rate.RateNotNull;
                label = account.Name;
                money = account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                reference = new Money(value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                id = account.Id;
            }
        }
    }
}
