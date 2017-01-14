using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
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

namespace MyCryptos.Forms.view.components
{
	public class RatesTableComponent : ContentView
	{
		private readonly HybridWebView webView;
		private readonly Label noDataLabel;
		private bool appeared;

		public RatesTableComponent(INavigation navigation)
		{
			var resolverContainer = new SimpleContainer();

			resolverContainer.Register<IJsonSerializer, JsonSerializer>();

			webView = new HybridWebView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				HeightRequest = 0
			};
			webView.RegisterCallback("Callback", code =>
			{
				var currency = CurrencyStorage.Find(code);

				Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new CoinInfoView(currency)));
			});

			webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
			{
				var size = int.Parse(sizeString);
				Device.BeginInvokeOnMainThread(() => webView.HeightRequest = size);
			});

			webView.RegisterCallback("HeaderClickedCallback", type =>
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
			stack.Children.Add(new ScrollView { Content = webView, IsClippedToBounds = true });
			Content = stack;

			UpdateView();

			Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);

			Messaging.RatesPageCurrency.SubscribeValueChanged(this, UpdateView);
			Messaging.Loading.SubscribeFinished(this, UpdateView);
		}

		public void OnAppearing()
		{
			if (!appeared)
			{
				appeared = true;
				webView.LoadFromContent("Html/ratesTable.html");
				Task.Delay(500).ContinueWith(t => UpdateView());
			}
			UpdateView();
		}

		public void UpdateView()
		{
			try
			{
				var items = ApplicationSettings.WatchedCurrencies
											   .Concat(ApplicationSettings.MainCurrencies)
							 					.Concat(AccountStorage.UsedCurrencies)
												.Distinct()
												.Where(c => !c.Equals(ApplicationSettings.SelectedRatePageCurrency))
												.Select(c => new Data(c)).ToList();

				var itemsExisting = (items.Count > 0);

				Device.BeginInvokeOnMainThread(() =>
				{
					noDataLabel.IsVisible = !itemsExisting;
					webView.IsVisible = itemsExisting;
				});

				if (!itemsExisting || !appeared) return;

				Func<Data, object> sortLambda;
				switch (ApplicationSettings.SortOrderRates)
				{
					case SortOrder.Alphabetical: sortLambda = d => d.Code; break;
					case SortOrder.ByUnits: sortLambda = d => decimal.Parse(Regex.Replace(d.Reference, "[A-Z]", "").Trim()); break;
					case SortOrder.ByValue: sortLambda = d => decimal.Parse(Regex.Replace(d.Reference, "[A-Z]", "").Trim()); break;
					case SortOrder.None: sortLambda = d => 1; break;
					default: sortLambda = d => 1; break;
				}

				items = ApplicationSettings.SortDirectionRates == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();

				webView.CallJsFunction("setHeader", new[]{
					new HeaderData(I18N.Currency, SortOrder.Alphabetical.ToString()),
					new HeaderData(string.Format(I18N.AsCurrency, ApplicationSettings.SelectedRatePageCurrency), SortOrder.ByValue.ToString())
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
			public readonly string Reference;

			public Data(Currency currency)
			{
				var neededRate = new ExchangeRate(currency, ApplicationSettings.SelectedRatePageCurrency);
				var rate = ExchangeRateHelper.GetRate(neededRate) ?? neededRate;

				Code = currency.Code;
				Reference = new Money(rate.RateNotNull, ApplicationSettings.SelectedRatePageCurrency).ToString8Digits(ApplicationSettings.RoundMoney);
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
