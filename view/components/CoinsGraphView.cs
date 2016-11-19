using System;
using System.Collections.Generic;
using System.Linq;
using constants;
using data.settings;
using data.storage;
using message;
using MyCryptos.helpers;
using MyCryptos.models;
using MyCryptos.resources;
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
                if (!element.Item1.Contains(InternationalisationResources.Others.Replace("{0}", string.Empty).Trim()))
                {
                    var currency = CurrencyStorage.Instance.AllElements.Find(e => e.Code.Equals(element.Item1));
                    Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new CoinDetailView(currency)));
                }
            });

            Content = WebView;

            HeightRequest = 500;

            updateView();

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView());
        }

        public void OnAppearing()
        {
            WebView.LoadFromContent("Html/graph.html");
        }

        void updateView()
        {
            var items = graphItemsGrouped.ToList();

            if (items.Count > 0)
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
                var smallItems = items.Where(e => (e.Item2 / reference) < AppConstants.PieGroupThreshold).ToList();

                if (smallItems.Count > 1)
                {
                    items = items.Where(e => !smallItems.Contains(e));
                    var grouped = Tuple.Create(string.Format(InternationalisationResources.Others, smallItems.Count), smallItems.Sum(e => e.Item2));

                    return items.Concat(new List<Tuple<string, decimal>> { grouped });
                }
                return items;
            }
        }
    }
}
