using MyCryptos.models;
using MyCryptos.resources;
using Xamarin.Forms;
using System.Collections.Generic;
using System;
using data.settings;
using System.Linq;
using MyCryptos.helpers;
using helpers;

namespace MyCryptos.view.components
{
	public class ReferenceCurrenciesSection
	{
		public List<ReferenceValueViewCell> Cells { get; private set; }
		public TableSection Section { get; private set; }

		public ReferenceCurrenciesSection(Money baseMoney)
		{
			if (baseMoney == null)
			{
				throw new ArgumentNullException();
			}

            var currencies = ApplicationSettings.ReferenceCurrencies;

			Section = new TableSection { Title = InternationalisationResources.EqualTo };
			Cells = new List<ReferenceValueViewCell>();

			foreach (var c in currencies)
			{
				var e = new ExchangeRate(baseMoney.Currency, c);
				var r = ExchangeRateHelper.GetRate(e);
				var cell = new ReferenceValueViewCell { ExchangeRate = r ?? e, Money = baseMoney };
				cell.IsLoading = (r != null && !r.Rate.HasValue);

				Cells.Add(cell);
				Section.Add(cell);
			}
		}
	}
}