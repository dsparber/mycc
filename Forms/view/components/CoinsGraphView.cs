using System;
using System.Collections.Generic;
using System.Linq;
using constants;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;
using view;

namespace MyCryptos.view
{
    public class CoinsGraphView : ContentView
    {
        HybridWebView WebView;
        private Label noCoinsLabel;
        private bool appeared;

        public CoinsGraphView(INavigation navigation)
        {
            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<IJsonSerializer, JsonSerializer>();

            WebView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                MinimumHeightRequest = 500
            };
            WebView.RegisterCallback("selectedCallback", t =>
            {
                var element = graphItemsGrouped.ToArray()[Convert.ToInt32(t)];
                if (!element.Item1.Contains(I18N.Others.Replace("{0}", string.Empty).Trim()))
                {
                    var currency = CurrencyStorage.Instance.AllElements.Find(e => e.Code.Equals(element.Item1));
                    Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new CoinDetailView(currency)));
                }
            });

            noCoinsLabel = new Label { Text = I18N.NoCoins, IsVisible = false, TextColor = AppConstants.FontColorLight, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            var stack = new StackLayout();
            stack.Children.Add(noCoinsLabel);
            stack.Children.Add(WebView);
            Content = stack;

            HeightRequest = 500;

            updateView();

            Messaging.UpdatingExchangeRates.SubscribeFinished(this, updateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, updateView);
            Messaging.ReferenceCurrency.SubscribeFinished(this, updateView);
            Messaging.Loading.SubscribeFinished(this, updateView);
        }

        public void OnAppearing()
        {
            if (appeared) return;

            appeared = true;
            WebView.LoadFromContent("Html/graph.html");
            updateView();
        }

        void updateView()
        {
            var items = graphItemsGrouped.ToList();
            var itemsExisting = (items.Count > 0);

            noCoinsLabel.IsVisible = !itemsExisting;
            WebView.IsVisible = itemsExisting;

            if (itemsExisting)
            {
                var c = AppConstants.BackgroundColor;
                WebView.CallJsFunction("displayGraph", items.Select(e => e.Item1).ToArray(), items.Select(e => e.Item2).ToArray(), string.Format("rgba({0},{1},{2},{3})", c.R * 255, c.G * 255, c.B * 255, c.A));
            }
        }

        IEnumerable<IGrouping<Currency, Account>> groups
        {
            get
            {
                var allAccounts = AccountStorage.Instance.AllElements;
                return allAccounts.GroupBy(a => a.Money.Currency);
            }
        }

        IEnumerable<Tuple<string, decimal>> graphItems
        {
            get
            {
                var elements = groups.Select(i =>
                {
                    var neededRate = new ExchangeRate(i.Key, ApplicationSettings.BaseCurrency);
                    var rate = ExchangeRateHelper.GetRate(neededRate);
                    if (rate != null && rate.Rate.HasValue)
                    {
                        return Tuple.Create(i.First().Money.Currency.Code, i.Sum(e => e.Money.Amount * rate.Rate.Value));
                    }
                    return null;
                });
                return elements.Where(e => e != null).OrderByDescending(e => e.Item2);
            }
        }

        IEnumerable<Tuple<string, decimal>> graphItemsGrouped
        {
            get
            {
                var items = graphItems;
                var reference = items.Sum(e => e.Item2);
                if (reference == 0)
                {
                    return new List<Tuple<string, decimal>>();
                }

                var smallItems = items.Where(e => (e.Item2 / reference) < AppConstants.PieGroupThreshold).ToList();

                if (smallItems.Count > 1)
                {
                    items = items.Where(e => !smallItems.Contains(e));
                    var grouped = Tuple.Create(string.Format(I18N.Others, smallItems.Count), smallItems.Sum(e => e.Item2));

                    return items.Concat(new List<Tuple<string, decimal>> { grouped });
                }
                return items;
            }
        }
    }
}
