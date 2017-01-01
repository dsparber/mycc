using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using constants;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.Resources;
using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.pages;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;
using CoinDetailView = MyCryptos.Forms.view.pages.CoinDetailView;

namespace MyCryptos.Forms.view.components
{
	public class CoinTableComponent : ContentView
	{
		private readonly HybridWebView webView;
		private readonly Label noDataLabel;
		private bool appeared;

		public CoinTableComponent(INavigation navigation)
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
			webView.RegisterCallback("Callback", code =>
			{
				var currency = CurrencyStorage.Find(code);
				var accounts = AccountStorage.AccountsWithCurrency(currency);

				Device.BeginInvokeOnMainThread(
					() =>
						navigation.PushAsync((accounts.Count == 1)
							? (Page)new AccountDetailView(accounts[0])
							: new CoinDetailView(currency)));
			});

			webView.RegisterCallback("HeaderClickedCallback", type =>
			{
				SortOrder value;
				var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
				if (clickedSortOrder == ApplicationSettings.SortOrderTable)
				{
					ApplicationSettings.SortDirectionTable = ApplicationSettings.SortDirectionTable == SortDirection.Ascending
						? SortDirection.Descending
						: SortDirection.Ascending;
				}
				ApplicationSettings.SortOrderTable = clickedSortOrder;

				UpdateView();
			});

			noDataLabel = new Label
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
			stack.Children.Add(noDataLabel);
			stack.Children.Add(webView);
			Content = stack;
			HeightRequest = 500;

			UpdateView();

			Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
			Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);

			Messaging.RoundNumbers.SubscribeValueChanged(this, UpdateView);
			Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
			Messaging.Loading.SubscribeFinished(this, UpdateView);
		}

		public void OnAppearing()
		{
			if (!appeared)
			{
				appeared = true;
				webView.LoadFromContent("Html/coinTable.html");
				Task.Delay(200).ContinueWith(t => UpdateView());
				Task.Delay(500).ContinueWith(t => UpdateView());
			}
			UpdateView();
		}

		public void UpdateView()
		{
			try
			{
				var items = AccountStorage.UsedCurrencies.Select(c => new Data(c)).ToList();
				var itemsExisting = (items.Count > 0);

				Device.BeginInvokeOnMainThread(() =>
				{
					noDataLabel.IsVisible = !itemsExisting;
					webView.IsVisible = itemsExisting;
				});

				if (!itemsExisting || !appeared) return;

				Func<Data, object> sortLambda;
				switch (ApplicationSettings.SortOrderTable)
				{
					case SortOrder.Alphabetical: sortLambda = d => d.Code; break;
					case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Amount.Replace("<", string.Empty)); break;
					case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Reference.Replace("<", string.Empty)); break;
					case SortOrder.None: sortLambda = d => 1; break;
					default: sortLambda = d => 1; break;
				}

				items = ApplicationSettings.SortDirectionTable == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();

				webView.CallJsFunction("setHeader", new[]{
					new HeaderData(I18N.Currency, SortOrder.Alphabetical.ToString()),
					new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString()),
					new HeaderData(string.Format(I18N.AsCurrency, ApplicationSettings.BaseCurrency.Code), SortOrder.ByValue.ToString())
				}, string.Empty);
				webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);
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
			public readonly string Code;
			[DataMember]
			public readonly string Name;
			[DataMember]
			public readonly string Amount;
			[DataMember]
			public readonly string Reference;

			public Data(Currency currency)
			{
				var sum = AccountStorage.AccountsWithCurrency(currency).Sum(a => a.Money.Amount);
				var neededRate = new ExchangeRate(currency, ApplicationSettings.BaseCurrency);
				var rate = ExchangeRateHelper.GetRate(neededRate) ?? neededRate;

				Code = currency.Code;
				Amount = new Money(sum, currency).ToStringTwoDigits(ApplicationSettings.RoundMoney, false).Replace(" ", string.Empty);
				Reference = new Money(sum * rate.RateNotNull, currency).ToStringTwoDigits(ApplicationSettings.RoundMoney, false).Replace(" ", string.Empty);
				Name = currency.Name;
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
				Direction = ApplicationSettings.SortDirectionTable.ToString();
				Type = ApplicationSettings.SortOrderTable.ToString();
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