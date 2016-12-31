using System;
using System.Collections.Generic;
using System.Linq;
using constants;
using MyCryptos.Core.Account.Storage;
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
using MyCryptos.Forms.view.pages;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MyCryptos.Forms.view.components
{
	public class CoinGraphComponent : ContentView
	{
		private readonly HybridWebView webView;
		private readonly Label noCoinsLabel;
		private bool appeared;

		public CoinGraphComponent(INavigation navigation)
		{
			var resolverContainer = new SimpleContainer();

			resolverContainer.Register<IJsonSerializer, JsonSerializer>();

			webView = new HybridWebView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				MinimumHeightRequest = 500
			};
			webView.RegisterCallback("selectedCallback", t =>
			{
				var element = GraphItemsGrouped.ToArray()[Convert.ToInt32(t)];
				if (element.Item1.Contains(I18N.Others.Replace("{0}", string.Empty).Trim())) return;

				var currency = CurrencyStorage.Instance.AllElements.Find(e => element.Item1.Equals(e?.Code));
				var accounts = AccountStorage.AccountsWithCurrency(currency);

				Device.BeginInvokeOnMainThread(() => navigation.PushAsync((accounts.Count == 1) ? (Page)new AccountDetailView(accounts[0]) : new CoinDetailView(currency)));
			});

			webView.RegisterCallback("appeared", t => UpdateView());


			noCoinsLabel = new Label { Text = I18N.NoDataToDisplay, IsVisible = false, TextColor = AppConstants.FontColorLight, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

			var stack = new StackLayout();
			stack.Children.Add(noCoinsLabel);
			stack.Children.Add(webView);
			Content = stack;

			HeightRequest = 500;

			UpdateView();

			Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
			Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);

			Messaging.ReferenceCurrency.SubscribeFinished(this, UpdateView);
			Messaging.Loading.SubscribeFinished(this, UpdateView);
		}

		public void OnAppearing()
		{
			if (!appeared)
			{
				appeared = true;
				webView.LoadFromContent("Html/graph.html");
				Task.Delay(500).ContinueWith(t => UpdateView());
			}
			UpdateView();
		}

		public void UpdateView()
		{
			try
			{
				var items = GraphItemsGrouped.ToList();
				var itemsExisting = (items.Count > 0);

				Device.BeginInvokeOnMainThread(() =>
				{
					noCoinsLabel.IsVisible = !itemsExisting;
					webView.IsVisible = itemsExisting;
				});

				if (!itemsExisting) return;

				var c = AppConstants.BackgroundColor;
				webView.CallJsFunction("displayGraph", items.Select(e => e.Item1).ToArray(), items.Select(e => e.Item2).ToArray(), $"rgba({c.R * 255},{c.G * 255},{c.B * 255},{c.A})");
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
		}

		private static IEnumerable<Tuple<string, decimal>> GraphItems
		{
			get
			{
				var elements = AccountStorage.AccountsGroupedByCurrency.Select(i =>
				{
					var neededRate = new ExchangeRate(i.Key, ApplicationSettings.BaseCurrency);
					var rate = ExchangeRateHelper.GetRate(neededRate);

					return rate?.Rate != null ? Tuple.Create(i.First().Money.Currency.Code, i.Sum(e => e.Money.Amount * rate.Rate.Value)) : null;
				});
				return elements.Where(e => e != null).OrderByDescending(e => e.Item2);
			}
		}

		private static IEnumerable<Tuple<string, decimal>> GraphItemsGrouped
		{
			get
			{
				var items = GraphItems;
				var graphItemsGrouped = items as IList<Tuple<string, decimal>> ?? items.ToList();

				var reference = graphItemsGrouped.Sum(e => e.Item2);
				if (reference == 0)
				{
					return new List<Tuple<string, decimal>>();
				}

				var smallItems = graphItemsGrouped.Where(e => (e.Item2 / reference) < AppConstants.PieGroupThreshold).ToList();

				if (smallItems.Count <= 1) return graphItemsGrouped;

				items = graphItemsGrouped.Where(e => !smallItems.Contains(e));
				var grouped = Tuple.Create(string.Format(I18N.Others, smallItems.Count), smallItems.Sum(e => e.Item2));

				return items.Concat(new List<Tuple<string, decimal>> { grouped });
			}
		}
	}
}
