using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.view.pages;
using Newtonsoft.Json;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;

namespace MyCC.Forms.view.components
{
    public class CoinGraphComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private bool _appeared;
        private bool _sizeAllocated;

        public CoinGraphComponent(INavigation navigation)
        {
            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>();

            _webView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                MinimumHeightRequest = 500
            };
            _webView.RegisterCallback("selectedCallback", id =>
            {
                var element = AccountStorage.Instance.AllElements.Find(e => e.Id == Convert.ToInt32(id));

                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new AccountDetailView(element)));
            });
            _webView.RegisterCallback("sizeAllocated", id =>
            {
                _sizeAllocated = true;
            });

            Content = _webView;
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
            if (_appeared) return;

            _appeared = true;
            _webView.LoadFromContent("Html/pieChart.html");

            Task.Factory.StartNew(async () =>
            {
                while (!_sizeAllocated)
                {
                    UpdateView();
                    await Task.Delay(200);
                }
            });
        }

        private void UpdateView()
        {
            try
            {
                var items = AccountStorage.AccountsGroupedByCurrency.Select(e => new Data(e, ApplicationSettings.BaseCurrency)).Where(d => d.value > 0).OrderByDescending(d => d.value).ToArray();
                Device.BeginInvokeOnMainThread(() => _webView.CallJsFunction("showChart", items, new string[] { I18N.OneAccount, I18N.Accounts }, new string[] { I18N.OneCurrency, I18N.Currencies }, I18N.Further, I18N.NoDataToDisplay));
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

                var totalMoney = new Money(group.Sum(a => a.Money.Amount), group.Key);

                label = group.Key.Code;
                name = group.Key.Name;
                value = totalMoney.Amount * rate.Rate ?? 0;
                money = totalMoney.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                reference = new Money(value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                accounts = group.Where(a => a.Money.Amount > 0).Select(a => new AccountData(a, rate, referenceCurrency)).OrderByDescending(d => d.value).ToArray();
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
                value = account.Money.Amount * rate.Rate ?? 0;
                label = account.Name;
                money = account.Money.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                reference = new Money(value, referenceCurrency).ToStringTwoDigits(ApplicationSettings.RoundMoney);
                id = account.Id;
            }
        }
    }
}
