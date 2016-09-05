using System.Collections.Generic;
using models;
using Xamarin.Forms;
using System.Linq;
using MyCryptos.resources;
using view.components;
using helpers;
using message;

namespace view
{
	public partial class CoinDetailView : ContentPage
	{
		public CoinDetailView(IEnumerable<Account> accounts, ExchangeRate exchangeRate)
		{
			InitializeComponent();

			var moneySum = new Money(accounts.Sum(a => a.Money.Amount), accounts.First().Money.Currency);

			Title = accounts.First().Money.Currency.Code;
			Header.TitleText = moneySum.ToString();

			if (exchangeRate.Rate.HasValue)
			{
				var moneyReference = new Money(moneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				Header.InfoText = moneyReference.ToString();
			}
			else {
				Header.InfoText = InternationalisationResources.NoExchangeRateFound;
			}

			var cells = new List<AccountViewCell>();
			foreach (var a in accounts)
			{
				cells.Add(new AccountViewCell(Navigation) { Account = a });
			}
			cells = SortHelper.SortCells(cells);
			foreach (var c in cells)
			{
				AccountSection.Add(c);
			}

			MessagingCenter.Subscribe<string>(this, MessageConstants.SortOrderChanged, (str) =>
			{
				cells = SortHelper.SortCells(cells);
				AccountSection.Clear();
				foreach (var c in cells)
				{
					AccountSection.Add(c);
				}
			});
		}
	}
}

