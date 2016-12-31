using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.view.components.cells;

namespace MyCryptos.Forms.view.components
{
	public class ReferenceCurrenciesSection
	{
		public List<ReferenceValueViewCell> Cells { get; }

		public ReferenceCurrenciesSection(Money baseMoney)
		{
			if (baseMoney == null)
			{
				throw new ArgumentNullException();
			}

			var currencies = ApplicationSettings.AllReferenceCurrencies.Where(c => !baseMoney.Currency.Equals(c));

			Cells = new List<ReferenceValueViewCell>();

			foreach (var c in currencies)
			{
				var e = new ExchangeRate(baseMoney.Currency, c);
				var r = ExchangeRateHelper.GetRate(e);
				var cell = new ReferenceValueViewCell
				{
					ExchangeRate = r ?? e,
					Money = baseMoney,
				};

				Cells.Add(cell);
			}
		}
	}
}