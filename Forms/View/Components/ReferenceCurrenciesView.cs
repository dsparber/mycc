using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;

namespace MyCC.Forms.View.Components
{
    public class ReferenceCurrenciesView : ContentView
    {
        public Money ReferenceMoney { get; set; }

        private readonly HybridWebView _webView;
        private readonly Label _lastUpdateLabel;
        private bool _appeared;

        private string TableHeaderLabel => string.Format(I18N.IsEqualTo, ReferenceMoney);
        private IEnumerable<Currency> ReferenceCurrencies => ApplicationSettings.AllReferenceCurrencies.Except(new List<Currency> { ReferenceMoney?.Currency });

        public ReferenceCurrenciesView(Money reference)
        {
            ReferenceMoney = reference;

            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<IJsonSerializer, JsonSerializer>();

            _webView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = AppConstants.TableBackgroundColor,
                HeightRequest = 0
            };

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });

            _webView.RegisterCallback("HeaderClickedCallback", type =>
            {
                SortOrder value;
                var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
                if (clickedSortOrder == SortOrder.None) return;

                if (clickedSortOrder == ApplicationSettings.SortOrderReferenceValues)
                {
                    ApplicationSettings.SortDirectionReferenceValues = ApplicationSettings.SortDirectionReferenceValues == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
                ApplicationSettings.SortOrderReferenceValues = clickedSortOrder;

                UpdateView();
            });

            _lastUpdateLabel = new Label { HorizontalTextAlignment = TextAlignment.End, TextColor = AppConstants.TableSectionColor, FontSize = AppConstants.TableSectionFontSize, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End };
            var headingLabel = new Label
            {
                Text = (Device.OS == TargetPlatform.iOS) ? TableHeaderLabel.ToUpper() : TableHeaderLabel,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                FontSize = AppConstants.TableSectionFontSize,
                TextColor = AppConstants.TableSectionColor
            };
            var labelStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(15, 15, 15, 8)
            };
            labelStack.Children.Add(headingLabel);
            labelStack.Children.Add(_lastUpdateLabel);

            var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };

            stack.Children.Add(labelStack);
            stack.Children.Add(_webView);

            Content = stack;

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);

            Messaging.RatesPageCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }

        public void OnAppearing()
        {
            if (_appeared) return;

            _appeared = true;
            _webView.LoadFromContent("Html/equalsTable.html");
            _webView.LoadFinished = (sender, e) => UpdateView();
        }

        public void UpdateView()
        {
            try
            {
                var items = ReferenceCurrencies.Select(c => new Data(ReferenceMoney, c)).ToList();
                var itemsExisting = (items.Count > 0);

                if (!itemsExisting || !_appeared) return;

                Func<Data, object> sortLambda;
                switch (ApplicationSettings.SortOrderReferenceValues)
                {
                    case SortOrder.Alphabetical: sortLambda = d => d.Code; break;
                    case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Amount.Replace("< ", string.Empty)); break;
                    case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Amount.Replace("< ", string.Empty)); break;
                    case SortOrder.None: sortLambda = d => 1; break;
                    default: sortLambda = d => 1; break;
                }
                items = ApplicationSettings.SortDirectionReferenceValues == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();

                var updateTime = ReferenceCurrencies
                                    .Select(e => new ExchangeRate(ReferenceMoney.Currency, e))
                                    .SelectMany(ExchangeRateHelper.GetNeededRates)
                                    .Distinct()
                                    .Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).Min();

                Device.BeginInvokeOnMainThread(() =>
                {
                    _webView.CallJsFunction("setHeader", new[]{
                      new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString()),
                      new HeaderData($"{I18N.Currency[0]}.", SortOrder.Alphabetical.ToString())
                          }, string.Empty);
                    _webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), ReferenceMoney.Amount == 1);
                    _lastUpdateLabel.Text = updateTime.LastUpdateString();
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string Amount;
            [DataMember]
            public readonly string Code;
            [DataMember]
            public readonly string Rate;

            public Data(Money reference, Currency currency)
            {
                var neededRate = new ExchangeRate(reference.Currency, currency);
                var rate = ExchangeRateHelper.GetRate(neededRate) ?? neededRate;

                Code = currency.Code;
                var money = new Money(reference.Amount * rate.Rate ?? 0 * reference.Amount, currency);
                Amount = money.ToString8Digits(false);
                Rate = new Money(rate.Rate ?? 0, currency).ToString8Digits(false);
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
                Direction = ApplicationSettings.SortDirectionReferenceValues.ToString();
                Type = ApplicationSettings.SortOrderReferenceValues.ToString();
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
