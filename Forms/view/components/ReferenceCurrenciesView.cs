using System;
using System.Collections.Generic;
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

		private List<Currency> _currencies;
		private bool _showAmountInHeader;
		private string tableHeaderLabel => _showAmountInHeader ? string.Format(I18N.IsEqualTo, referenceMoney.ToString()) : I18N.EqualTo;
		private IEnumerable<Currency> referenceCurrencies => _currencies ?? ApplicationSettings.AllReferenceCurrencies.Where(c => !c.Equals(referenceMoney.Currency));

		public ReferenceCurrenciesView(Money reference, bool showAmountInHeader = false, List<Currency> currencies = null)
		{
			referenceMoney = reference;
			_currencies = currencies;
			_showAmountInHeader = showAmountInHeader;

			var resolverContainer = new SimpleContainer();

			resolverContainer.Register<IJsonSerializer, JsonSerializer>();

			webView = new HybridWebView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = AppConstants.TableBackgroundColor,
				HeightRequest = 0
			};

			webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
			{
				var size = int.Parse(sizeString);
				Device.BeginInvokeOnMainThread(() => webView.HeightRequest = size);
			});

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

			var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };

			stack.Children.Add(new Label { Text = (Device.OS == TargetPlatform.iOS) ? tableHeaderLabel.ToUpper() : tableHeaderLabel, HorizontalOptions = LayoutOptions.FillAndExpand, Margin = new Thickness(8, 24, 8, 8), FontSize = AppConstants.TableSectionFontSize, TextColor = AppConstants.TableSectionColor });
			stack.Children.Add(webView);

			Content = stack;

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
				UpdateView();

				Task.Delay(200).ContinueWith(t => UpdateView());
				Task.Delay(500).ContinueWith(t => UpdateView());
				Task.Delay(1000).ContinueWith(t => UpdateView());
				Task.Delay(1500).ContinueWith(t => UpdateView());
				Task.Delay(2000).ContinueWith(t => UpdateView());
			}
		}

		public void UpdateView()
		{
			try
			{
				var items = referenceCurrencies.Select(c => new Data(referenceMoney, c)).ToList();
				var itemsExisting = (items.Count > 0);
				Debug.WriteLine(111);

				if (!itemsExisting || !appeared) return;
				Debug.WriteLine(114);

				Func<Data, object> sortLambda;
				Debug.WriteLine(117);
				switch (ApplicationSettings.SortOrderReferenceValues)
				{
					case SortOrder.Alphabetical: sortLambda = d => d.Code; break;
					case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Amount.Replace("< ", string.Empty)); break;
					case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Amount.Replace("< ", string.Empty)); break;
					case SortOrder.None: sortLambda = d => 1; break;
					default: sortLambda = d => 1; break;
				}
				Debug.WriteLine(126);
				items = ApplicationSettings.SortDirectionReferenceValues == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();
				Debug.WriteLine(118);
				webView.CallJsFunction("setHeader", new[]{
					new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString()),
					new HeaderData($"{I18N.Currency[0]}.", SortOrder.Alphabetical.ToString())
				}, string.Empty);
				Debug.WriteLine(133);
				webView.CallJsFunction("updateTable", items.ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);
				Debug.WriteLine(135);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
			Debug.WriteLine(140);
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
				var money = new Money(rate.RateNotNull * reference.Amount, currency);
				Amount = money.ToString8Digits(false);
				Rate = new Money(rate.RateNotNull, currency).ToString8Digits(false);
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
