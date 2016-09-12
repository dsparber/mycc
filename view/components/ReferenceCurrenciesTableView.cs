using models;
using MyCryptos.resources;
using Xamarin.Forms;
using System.Collections.Generic;
using data.settings;
using System.Linq;
using data.storage;
using enums;

namespace view.components
{
	public class ReferenceCurrenciesTableView : TableView
	{
		public readonly List<ReferenceValueViewCell> Cells;
		public readonly TableSection Section;

		List<Currency> referenceCurrencies;
		Money baseMoney;

		public Money BaseMoney
		{
			get { return baseMoney; }
			set { baseMoney = value; setView(); }
		}

		public ReferenceCurrenciesTableView()
		{
			Section = new TableSection { Title = InternationalisationResources.EqualTo };
			Root.Add(Section);

			Cells = new List<ReferenceValueViewCell>();
			referenceCurrencies = new List<Currency>();
			referenceCurrencies.Add(ApplicationSettings.BaseCurrency);
			referenceCurrencies.Add(Currency.BTC);
			referenceCurrencies.Add(Currency.EUR);
			referenceCurrencies.Add(Currency.USD);
			referenceCurrencies = referenceCurrencies.Distinct().OrderBy(c => c.Code).ToList();

			setView();
		}

		void setView()
		{
			if (baseMoney != null)
			{
				Section.Clear();
				Cells.Clear();

				foreach (var c in referenceCurrencies)
				{
					var rate = new ExchangeRate(baseMoney.Currency, c);
					var rateFromStorage = ExchangeRateStorage.Instance.CachedElements.Find(r => r.Equals(rate));
					var cell = new ReferenceValueViewCell { ExchangeRate = rateFromStorage ?? rate, Money = baseMoney };
					Section.Add(cell);
					Cells.Add(cell);
				}
				updateView();
			}
		}

		async void updateView()
		{
			foreach (var cell in Cells)
			{
				if (cell.IsLoading)
				{
					var currency = (await CurrencyStorage.Instance.AllElements()).Find(e => e.Equals(cell.ExchangeRate.SecondaryCurrency));
					var rate = await ExchangeRateStorage.Instance.GetRate(baseMoney.Currency, currency, FetchSpeedEnum.MEDIUM);
					cell.ExchangeRate = rate;
					cell.IsLoading = false;
				}
			}
		}
	}
}

