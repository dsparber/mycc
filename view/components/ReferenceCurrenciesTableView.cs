using MyCryptos.models;
using MyCryptos.resources;
using Xamarin.Forms;
using System.Collections.Generic;
using data.settings;
using System.Linq;
using data.storage;

namespace MyCryptos.view.components
{
	public class ReferenceCurrenciesTableView : TableView
	{
		public List<ReferenceValueViewCell> Cells;
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
					if (baseMoney.Currency.Equals(c))
					{
						rate.Rate = 1;
					}
					var rateFromStorage = ExchangeRateStorage.Instance.Find(rate) ?? ((ExchangeRateStorage.Instance.Find(rate.Inverse) != null) ? ExchangeRateStorage.Instance.Find(rate.Inverse).Inverse : null);
					rate = rate.Rate.HasValue ? rate : rateFromStorage ?? rate;
					var cell = new ReferenceValueViewCell { ExchangeRate = rate, Money = baseMoney };

					Cells.Add(cell);
				}
			}
		}
	}
}