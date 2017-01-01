using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using constants;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.Resources;
using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;

namespace MyCryptos.Forms.view.components
{
	public class ReferenceCurrenciesView : ContentView
	{
		private readonly HybridWebView webView;
		private bool appeared;
		private Money referenceMoney;

		public ReferenceCurrenciesView(Money reference)
		{
			referenceMoney = reference;

			var resolverContainer = new SimpleContainer();

			resolverContainer.Register<IJsonSerializer, JsonSerializer>();

			webView = new HybridWebView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = AppConstants.TableBackgroundColor,
				HeightRequest = 500
			};

			webView.RegisterCallback("HeaderClickedCallback", type =>
			{
				SortOrder value;
				var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
				if (clickedSortOrder != SortOrder.None)
				{
					if (clickedSortOrder == ApplicationSettings.SortOrderReferenceValues)
					{
						ApplicationSettings.SortDirectionReferenceValues = ApplicationSettings.SortDirectionReferenceValues == SortDirection.Ascending
							? SortDirection.Descending
							: SortDirection.Ascending;
					}
					ApplicationSettings.SortOrderReferenceValues = clickedSortOrder;

					UpdateView();
				}
			});

			var label = string.Format(I18N.IsEqualTo, referenceMoney.ToString());

			var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };
			stack.Children.Add(new Label { Text = (Device.OS == TargetPlatform.iOS) ? label.ToUpper() : label, HorizontalOptions = LayoutOptions.FillAndExpand, Margin = new Thickness(15, 25, 8, 8), FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.TableSectionColor });
			stack.Children.Add(webView);

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
				webView.LoadFromContent("Html/equalsTable.html");
				Task.Delay(200).ContinueWith(t => UpdateView());
				Task.Delay(500).ContinueWith(t => UpdateView());
				Task.Delay(1000).ContinueWith(t => UpdateView());
			}
			UpdateView();
		}

		public void UpdateView()
		{
			try
			{
				var items = ApplicationSettings.AllReferenceCurrencies.Select(c => new Data(referenceMoney, c)).ToList();
				var itemsExisting = (items.Count > 0);

				if (!itemsExisting || !appeared) return;

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

				webView.CallJsFunction("setHeader", new[]{
					new HeaderData(I18N.Currency, SortOrder.Alphabetical.ToString()),
					new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString())
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
			public readonly string Amount;
			[DataMember]
			public readonly string Code;

			public Data(Money reference, Currency currency)
			{
				var neededRate = new ExchangeRate(reference.Currency, currency);
				var rate = ExchangeRateHelper.GetRate(neededRate) ?? neededRate;

				Code = currency.Code;
				var money = new Money(rate.RateNotNull * reference.Amount, currency);
				Amount = reference.Amount == 1 ? money.ToString8Digits(ApplicationSettings.RoundMoney, false) : money.ToStringTwoDigits(ApplicationSettings.RoundMoney, false);
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