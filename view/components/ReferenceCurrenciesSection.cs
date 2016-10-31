using MyCryptos.models;
using MyCryptos.resources;
using Xamarin.Forms;
using System.Collections.Generic;
using System;
using data.settings;
using System.Linq;
using MyCryptos.helpers;

namespace MyCryptos.view.components
{
	public class ReferenceCurrenciesSection 
	{
		public List<ReferenceValueViewCell> Cells { get; private set; }
		public TableSection Section { get; private set; }

		public ReferenceCurrenciesSection(Money baseMoney)
		{

			var exchangeRates = new List<ExchangeRate>();
			var currencies = ApplicationSettings.ReferenceCurrencies;
			currencies.Add(ApplicationSettings.BaseCurrency);
			currencies = currencies.Distinct().ToList();

			foreach (var c in currencies)
			{
				exchangeRates.Add(ExchangeRateHelper.GetRate(baseMoney.Currency, c));
			}

			if (exchangeRates == null || baseMoney == null)
			{
				throw new ArgumentNullException();
			}

			Section = new TableSection { Title = InternationalisationResources.EqualTo };
			Cells = new List<ReferenceValueViewCell>();

			foreach (var e in exchangeRates)
			{
				var cell = new ReferenceValueViewCell { ExchangeRate = e, Money = baseMoney };
				Cells.Add(cell);
				Section.Add(cell);
			}
		}
	}
}