using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Utils;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;
using MyCC.Forms.View.Components.BaseComponents;

namespace MyCC.Forms.View.Components.Table
{
    public class CoinTableComponent : ContentView
    {
        private readonly HybridWebView _webView;

        public CoinTableComponent()
        {
            _webView = new HybridWebView("Html/coinTable.html") { LoadFinished = UpdateView };

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });
            _webView.RegisterCallback("Callback", code =>
            {
                var currency = new Currency(code.Split(',')[0], bool.Parse(code.Split(',')[1]));
                currency = currency.Find();

                var accounts = AccountStorage.AccountsWithCurrency(currency);

                var view = accounts.Count == 1 ? (Page)new AccountView(accounts[0]) : new AccountGroupView(currency);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    foreach (var page in Navigation.NavigationStack.Where(p => !(p is AssetsTableView))) Navigation.RemovePage(page);
                    await Navigation.PushAsync(view);
                });
            });

            _webView.RegisterCallback("HeaderClickedCallback", type =>
            {
                SortOrder value;
                var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
                if (clickedSortOrder == ApplicationSettings.SortOrderAssets)
                {
                    ApplicationSettings.SortDirectionAssets = ApplicationSettings.SortDirectionAssets == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
                ApplicationSettings.SortOrderAssets = clickedSortOrder;

                UpdateView();
            });

            Content = _webView;

            _webView.LoadFinished = UpdateView;

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingRates.SubscribeFinished(this, UpdateView);

            Messaging.RoundNumbers.SubscribeValueChanged(this, UpdateView);
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }

        private void UpdateView()
        {
            try
            {
                var items = AccountStorage.UsedCurrencies.Select(c => new Data(c)).ToList();
                var itemsExisting = items.Count > 0;

                if (!itemsExisting) return;

                Func<Data, object> sortLambda;
                switch (ApplicationSettings.SortOrderAssets)
                {
                    case SortOrder.Alphabetical: sortLambda = d => d.Code; break;
                    case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Amount.Replace("<", string.Empty)); break;
                    case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Reference.Replace("<", string.Empty)); break;
                    case SortOrder.None: sortLambda = d => 1; break;
                    default: sortLambda = d => 1; break;
                }

                items = (ApplicationSettings.SortDirectionAssets == SortDirection.Ascending ? items.Where(d => !d.Disabled).OrderBy(sortLambda) : items.Where(d => !d.Disabled).OrderByDescending(sortLambda)).Concat
                        (ApplicationSettings.SortDirectionAssets == SortDirection.Ascending ? items.Where(d => d.Disabled).OrderBy(sortLambda) : items.Where(d => d.Disabled).OrderByDescending(sortLambda)).ToList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    _webView.CallJsFunction("setHeader", new[]{
                      new HeaderData(I18N.Currency, SortOrder.Alphabetical.ToString()),
                      new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString()),
                      new HeaderData(string.Format(I18N.AsCurrency, ApplicationSettings.StartupCurrencyAssets.ToCurrency().Code), SortOrder.ByValue.ToString())
                  }, string.Empty);
                    _webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);

                    if (Device.RuntimePlatform.Equals(Device.Android))
                    {
                        HeightRequest = 38 * (items.Count + 1) + 1;
                    }
                });
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string CallbackString;
            [DataMember]
            public readonly string Code;
            [DataMember]
            public readonly string Name;
            [DataMember]
            public readonly string Amount;
            [DataMember]
            public readonly string Reference;
            [DataMember]
            public readonly bool Disabled;

            public Data(string currencyId)
            {
                var sum = AccountStorage.AccountsWithCurrency(currencyId).Sum(a => a.IsEnabled ? a.Money.Amount : 0);
                var neededRate = new ExchangeRate(currencyId, ApplicationSettings.StartupCurrencyAssets);
                var rate = RateUtil.GetRate(neededRate) ?? neededRate;

                var currency = currencyId.ToCurrency();
                Code = currency.Code;
                Amount = new Money(sum, currency).ToStringTwoDigits(ApplicationSettings.RoundMoney, false).Replace(" ", string.Empty);
                Reference = new Money(sum * rate.Rate ?? 0, currency).ToStringTwoDigits(ApplicationSettings.RoundMoney, false).Replace(" ", string.Empty);
                Name = currency.Name;
                CallbackString = currency.Code + "," + currency.IsCrypto;
                Disabled = sum == 0;
            }

            public override string ToString()
            {
                return string.Format($"{Amount} {Code}\t= {Reference}");
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
                Direction = ApplicationSettings.SortDirectionAssets.ToString();
                Type = ApplicationSettings.SortOrderAssets.ToString();
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
