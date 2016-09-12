using System;
using System.Collections.Generic;
using System.Linq;
using data.settings;
using data.storage;
using enums;
using models;
using view.components;

namespace helpers
{
	public static class SortHelper
	{
		public static List<CoinViewCell> SortCells(List<CoinViewCell> cells)
		{
			Func<CoinViewCell, object> sortLambda;

			if (ApplicationSettings.SortOrder.Equals(SortOrder.BY_VALUE))
			{
				sortLambda = c => c.MoneyReference != null ? c.MoneyReference.Amount : 0;
			}
			else if (ApplicationSettings.SortOrder.Equals(SortOrder.BY_UNITS))
			{
				sortLambda = c => c.MoneySum.Amount;
			}
			else
			{
				sortLambda = c => c.Currency != null ? c.Currency.Code : string.Empty;
			}


			if (ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING))
			{
				cells = cells.OrderBy(sortLambda).ToList();
			}
			else {
				cells = cells.OrderByDescending(sortLambda).ToList();
			}

			return cells;
		}

		public static List<AccountViewCell> SortCells(List<AccountViewCell> cells)
		{
			Func<AccountViewCell, object> sortLambda;

			if (ApplicationSettings.SortOrder.Equals(SortOrder.BY_VALUE))
			{
				sortLambda = c =>
				{
					var rate = ExchangeRateStorage.Instance.CachedElements.Find(e => e.Equals(new ExchangeRate(c.Account.Money.Currency, ApplicationSettings.BaseCurrency)));
					return c.Account.Money.Amount * (rate != null ? rate.RateNotNull : 0);
				};
			}
			else if (ApplicationSettings.SortOrder.Equals(SortOrder.BY_UNITS))
			{
				sortLambda = c => c.Account.Money.Amount;
			}
			else
			{
				sortLambda = c => c.Account.Name + c.Account.Money.Currency.Code;
			}


			if (ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING))
			{
				cells = cells.OrderBy(sortLambda).ToList();
			}
			else {
				cells = cells.OrderByDescending(sortLambda).ToList();
			}

			return cells;
		}
	}
}

