using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.settings;
using MyCryptos.Forms.helpers;
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
			var currencyCells = new List<CustomViewCell>();

			var referenceCurrencies = new List<Currency>(ApplicationSettings.AllReferenceCurrencies).ToList();
			var mainCurrencies = new List<Currency>(ApplicationSettings.MainCurrencies).ToList();

			Header.InfoText = PluralHelper.GetTextCurrencies(ApplicationSettings.AllReferenceCurrencies.Count);

			foreach (var c in referenceCurrencies)
			{
				var isMainCurrency = mainCurrencies.Contains(c);

				var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
				var star = new CustomViewCellActionItem { Icon = isMainCurrency ? "starFilled.png" : "star.png", Data = c };
				var items = new List<CustomViewCellActionItem> { star, delete };

				delete.Action = (sender, e) =>
				{
					var cu = (e as TappedEventArgs)?.Parameter as Currency;
					mainCurrencies.RemoveAll(x => x.Equals(cu));
					referenceCurrencies.RemoveAll(x => x.Equals(cu));

					if (ApplicationSettings.BaseCurrency.Equals(cu))
					{
						ApplicationSettings.BaseCurrency = mainCurrencies.First();
						Messaging.ReferenceCurrency.SendValueChanged();
					}
					ApplicationSettings.MainCurrencies = mainCurrencies;
					ApplicationSettings.FurtherCurrencies = referenceCurrencies.Where(x => !mainCurrencies.Contains(x)).ToList();
					Messaging.ReferenceCurrencies.SendValueChanged();
					SetReferenceCurrencyCells();
				};

				star.Action = (sender, e) =>
				{
					var cu = (e as TappedEventArgs)?.Parameter as Currency;

					var isMain = mainCurrencies.Contains(cu);

					if (isMain)
					{
						mainCurrencies.Remove(cu);
					}
					else {
						mainCurrencies.Add(cu);
					}

					if (isMain && ApplicationSettings.BaseCurrency.Equals(cu))
					{
						ApplicationSettings.BaseCurrency = mainCurrencies.First();
						Messaging.ReferenceCurrency.SendValueChanged();
					}

					ApplicationSettings.MainCurrencies = mainCurrencies;
					ApplicationSettings.FurtherCurrencies = referenceCurrencies.Where(x => !mainCurrencies.Contains(x)).ToList();
					Messaging.ReferenceCurrencies.SendValueChanged();
					SetReferenceCurrencyCells();
				};


				if (c.Equals(Currency.Btc))
				{
					star.Action = (sender, e) => DisplayAlert(I18N.Error, I18N.BitcoinCanNotBeUnstared, I18N.Ok);
					delete.Action = (sender, e) => DisplayAlert(I18N.Error, I18N.BitcoinCanNotBeRemoved, I18N.Ok);
				}

				var cell = new CustomViewCell
				{
					Text = c.Code,
					ActionItems = items,
					Detail = c.Name
				};

				currencyCells.Add(cell);
			}

			currencyCells = currencyCells.OrderBy(c => c.Text).ToList();

			//var newCells = currencyCells.Where(x => !CurrenciesSection.Contains(x)); // TODO Implement Equals for cells needed for Contains
			//var oldCells = CurrenciesSection.Where(x => !currencyCells.Contains(x));
			//oldCells.Select(CurrenciesSection.Remove);
			//CurrenciesSection.Add(newCells);
			CurrenciesSection.Clear();
			CurrenciesSection.Add(currencyCells);

			var allReferenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToArray();
			var currencies = CurrencyStorage.Instance.AllElements.Where(c => !allReferenceCurrencies.Contains(c)).ToList();

			var addCurrencyCell = new CustomViewCell { Text = I18N.AddReferenceCurrency, IsActionCell = true };
			addCurrencyCell.Tapped += (sender, e) =>
			{
				var overlay = new CurrencyOverlay(currencies)
				{
					CurrencySelected = (c) =>
					{
						referenceCurrencies.Add(c);
						ApplicationSettings.FurtherCurrencies = referenceCurrencies.Where(x => !mainCurrencies.Contains(x)).ToList();
						Messaging.ReferenceCurrencies.SendValueChanged();
						SetReferenceCurrencyCells();
					}
				};

				Navigation.PushAsync(overlay);
			};

			CurrenciesSection.Add(addCurrencyCell);
		}
	}
}
