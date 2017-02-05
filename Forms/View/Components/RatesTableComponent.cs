using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;

namespace MyCC.Forms.View.Components
{
    public class RatesTableComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private readonly Label _noDataLabel;
        private bool _appeared;

        public RatesTableComponent(INavigation navigation)
        {
            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<IJsonSerializer, JsonSerializer>();

            _webView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                HeightRequest = 0
            };
            _webView.RegisterCallback("Callback", code =>
            {
                var currency = new Currency(code.Split(',')[0], bool.Parse(code.Split(',')[1]));
                currency = CurrencyStorage.Instance.Find(currency) ?? currency;

                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new CoinInfoView(currency)));
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

            _noDataLabel = new Label
            {
                Text = I18N.NoDataToDisplay,
                IsVisible = false,
                TextColor = AppConstants.FontColorLight,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            stack.Children.Add(_noDataLabel);
            stack.Children.Add(new ScrollView { Content = _webView, IsClippedToBounds = true });
            Content = stack;

            UpdateView();

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);

            Messaging.RatesPageCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }

        public void OnAppearing()
        {
            if (!_appeared)
            {
                _appeared = true;
                _webView.LoadFromContent("Html/ratesTable.html");
                _webView.LoadFinished = (sender, e) => UpdateView();

            }
        }

        private void UpdateView()
        {
            if (_appeared)
            {
                var items = ApplicationSettings.WatchedCurrencies
                                               .Concat(ApplicationSettings.AllReferenceCurrencies)
                                                 .Concat(AccountStorage.UsedCurrencies)
                                                .Distinct()
                                                .Where(c => !c.Equals(ApplicationSettings.SelectedRatePageCurrency))
                                                .Select(c => new Data(c)).ToList();

                var itemsExisting = (items.Count > 0);

                Device.BeginInvokeOnMainThread(() =>
                {
                    _noDataLabel.IsVisible = !itemsExisting;
                    _webView.IsVisible = itemsExisting;
                });

                if (!itemsExisting || !_appeared) return;

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
                      new HeaderData(string.Format(I18N.AsCurrency, ApplicationSettings.SelectedRatePageCurrency), SortOrder.ByValue.ToString())
                          }, string.Empty);
                    _webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);
                });
            }
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
                var neededRate = new ExchangeRate(currency, ApplicationSettings.SelectedRatePageCurrency);
                var rate = ExchangeRateHelper.GetRate(neededRate) ?? neededRate;

                Code = currency.Code;
                Reference = new Money(rate.Rate ?? 0, ApplicationSettings.SelectedRatePageCurrency).ToString8Digits(false);
                CallbackString = currency.Code + "," + currency.IsCryptoCurrency;
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
    }
}
