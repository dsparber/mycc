using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.overlays;
using MyCryptos.view.components;
using MyCryptos.view.components.cells;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages.settings
{
	public partial class ReferenceCurrenciesSettingsView
	{
		public ReferenceCurrenciesSettingsView()
		{
			InitializeComponent();
			SetReferenceCurrencyCells();

			Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetReferenceCurrencyCells);
		}

		private void SetReferenceCurrencyCells()
		{
			var mainCurrencyCells = new List<CustomViewCell>();
			var furtherCurrencyCells = new List<CustomViewCell>();

			var mainCurrencies = new List<Currency>(ApplicationSettings.MainReferenceCurrencies);

			foreach (var c in mainCurrencies)
			{
				var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
				var items = new List<CustomViewCellActionItem> { delete };

				delete.Action = (sender, e) =>
				{
					var cu = (e as TappedEventArgs)?.Parameter as Currency;
					mainCurrencies.Remove(cu);
					if (ApplicationSettings.BaseCurrency.Equals(cu))
					{
						ApplicationSettings.BaseCurrency = mainCurrencies[0];
						Messaging.ReferenceCurrency.SendValueChanged();
					}
					ApplicationSettings.MainReferenceCurrencies = mainCurrencies;
					Messaging.ReferenceCurrencies.SendValueChanged();
					SetReferenceCurrencyCells();
				};

				if (mainCurrencies.Count <= 1)
				{
					items.Remove(delete);
				}

				var cell = new CustomViewCell
				{
					Text = c.Code,
					ActionItems = items,
					Detail = c.Name
				};

				mainCurrencyCells.Add(cell);
			}

			var furtherCurrencies = new List<Currency>(ApplicationSettings.FurtherReferenceCurrencies);

			foreach (var c in furtherCurrencies)
			{
				var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
				var items = new List<CustomViewCellActionItem> { delete };

				delete.Action = (sender, e) =>
				{
					var cu = (e as TappedEventArgs)?.Parameter as Currency;
					furtherCurrencies.Remove(cu);
					ApplicationSettings.FurtherReferenceCurrencies = furtherCurrencies;
					Messaging.ReferenceCurrencies.SendValueChanged();
					SetReferenceCurrencyCells();
				};

				var cell = new CustomViewCell
				{
					Text = c.Code,
					ActionItems = items,
					Detail = c.Name
				};

				furtherCurrencyCells.Add(cell);
			}

			MainCurrenciesSection.Clear();
			AllCurrenciesSection.Clear();
			MainCurrenciesSection.Add(mainCurrencyCells);
			AllCurrenciesSection.Add(furtherCurrencyCells);


			var allReferenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToArray();
			var currencies = CurrencyStorage.Instance.AllElements.Where(c => !allReferenceCurrencies.Contains(c)).ToList();

			var addCurrencyCell = new CustomViewCell { Text = I18N.AddMainCurrencies, IsActionCell = true };
			addCurrencyCell.Tapped += (sender, e) =>
			{
				var overlay = new CurrencyOverlay(currencies)
				{
					CurrencySelected = (c) =>
					{
						mainCurrencies.Add(c);
						ApplicationSettings.MainReferenceCurrencies = mainCurrencies;
						Messaging.ReferenceCurrencies.SendValueChanged();
						SetReferenceCurrencyCells();
					}
				};

				Navigation.PushAsync(overlay);
			};
			if (mainCurrencies.Count < 3)
			{
				MainCurrenciesSection.Add(addCurrencyCell);
			}

			var addfurtherCurrencyCell = new CustomViewCell { Text = I18N.AddFurtherCurrencies, IsActionCell = true };
			addfurtherCurrencyCell.Tapped += (sender, e) =>
			{
				var overlay = new CurrencyOverlay(currencies)
				{
					CurrencySelected = (c) =>
					{
						furtherCurrencies.Add(c);
						ApplicationSettings.FurtherReferenceCurrencies = furtherCurrencies;
						Messaging.ReferenceCurrencies.SendValueChanged();
						SetReferenceCurrencyCells();
					}
				};

				Navigation.PushAsync(overlay);
			};
			AllCurrenciesSection.Add(addfurtherCurrencyCell);
		}
	}
}
