using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Table
{
    public class RatesTableComponent : ContentView
    {
        private readonly HybridWebView _webView;

        public RatesTableComponent(INavigation navigation)
        {
            _webView = new HybridWebView("Html/ratesTable.html")
            {
                LoadFinished = UpdateView
            };
            _webView.RegisterCallback("Callback", code =>
            {
                var currency = new Currency(code.Split(',')[0], bool.Parse(code.Split(',')[1]));
                currency = currency.Find();

                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new CoinInfoView(currency, true)));
            });

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });

            _webView.RegisterCallback("HeaderClickedCallback", type =>
            {
                SortOrder value;
                var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
                if (clickedSortOrder == ApplicationSettings.SortOrderRates)
                {
                    ApplicationSettings.SortDirectionRates = ApplicationSettings.SortDirectionRates == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
                ApplicationSettings.SortOrderRates = clickedSortOrder;

                UpdateView();
            });

            Content = _webView;

            _webView.LoadFinished = UpdateView;

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);

            Messaging.RatesPageCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }

        public void OnAppearing()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            var items = ApplicationSettings.WatchedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .Concat(AccountStorage.UsedCurrencies)
                .Distinct()
                .Where(c => !c.Equals(ApplicationSettings.StartupCurrencyRates))
                .Select(c => new Data(c.ToCurrency())).ToList();

            var itemsExisting = items.Count > 0;

            if (!itemsExisting) return;

            Func<Data, object> sortLambda;
            switch (ApplicationSettings.SortOrderRates)
            {
                case SortOrder.Alphabetical: sortLambda = d => d.Code; break;
                case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Reference); break;
                case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Reference); break;
                case SortOrder.None: sortLambda = d => 1; break;
                default: sortLambda = d => 1; break;
            }

            items = ApplicationSettings.SortDirectionRates == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();

            Device.BeginInvokeOnMainThread(() =>
            {
                _webView.CallJsFunction("setHeader", new[]{
                    new HeaderData(I18N.Currency, SortOrder.Alphabetical.ToString()),
                    new HeaderData(string.Format(I18N.AsCurrency, ApplicationSettings.StartupCurrencyRates.ToCurrency().Code), SortOrder.ByValue.ToString())
                }, string.Empty);
                _webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);

                if (Device.RuntimePlatform.Equals(Device.Android))
                {
                    HeightRequest = 38 * (items.Count + 1) + 1;
                }
            });
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string Code;
            [DataMember]
            public readonly string Reference;
            [DataMember]
            public readonly string CallbackString;

            public Data(Currency currency)
            {
                var neededRate = new ExchangeRate(currency.Id, ApplicationSettings.StartupCurrencyRates);
                var rate = RateHelper.GetRate(neededRate) ?? neededRate;

                Code = currency.Code;
                Reference = new Money(rate.Rate ?? 0, ApplicationSettings.StartupCurrencyRates.ToCurrency()).ToString8Digits(false);
                CallbackString = currency.Code + "," + currency.IsCrypto;
            }

            public override string ToString()
            {
                return string.Format($"{Code}: {Reference}");
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class SortData
        {
            [DataMember]
            public readonly string Direction;
            [DataMember]
            public readonly string Type;

            public SortData()
            {
                Direction = ApplicationSettings.SortDirectionRates.ToString();
                Type = ApplicationSettings.SortOrderRates.ToString();
            }

        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class HeaderData
        {
            [DataMember]
            public readonly string Text;
            [DataMember]
            public readonly string Type;

            public HeaderData(string text, string type)
            {
                Text = text;
                Type = type;
            }

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateView();
        }
    }
}
