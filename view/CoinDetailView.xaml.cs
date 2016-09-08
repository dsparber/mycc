using System.Collections.Generic;
using models;
using Xamarin.Forms;
using System.Linq;
using MyCryptos.resources;
using view.components;
using helpers;
using message;
using data.repositories.account;
using System;
using data.storage;

namespace view
{
	public partial class CoinDetailView : ContentPage
	{
		public CoinDetailView(IEnumerable<Tuple<Account, AccountRepository>> accounts, ExchangeRate exchangeRate)
		{
			InitializeComponent();
			var currency = accounts.First().Item1.Money.Currency;

			updateView(accounts, exchangeRate);

			MessagingCenter.Subscribe<string>(this, MessageConstants.SortOrderChanged, (str) =>
			{
				updateView(accounts, exchangeRate);
			});

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdateAccounts, async (str) =>
			{
				var accs = await AccountStorage.Instance.AllElementsWithRepositories();
				accs = accs.Where(t => t.Item1.Money.Currency.Equals(currency)).ToList();

				if (accs.Count == 0)
				{
					Navigation.RemovePage(this);
				}
				else {
					updateView(accs, exchangeRate);
				}
			});
		}

		void updateView(IEnumerable<Tuple<Account, AccountRepository>> accounts, ExchangeRate exchangeRate)
		{
			var cells = new List<AccountViewCell>();
			foreach (var a in accounts)
			{
				cells.Add(new AccountViewCell(Navigation) { Account = a.Item1, Repository = a.Item2 });
			}

			cells = SortHelper.SortCells(cells);
			AccountSection.Clear();
			foreach (var c in cells)
			{
				AccountSection.Add(c);
			}

			setHeader(accounts, exchangeRate);
		}

		void setHeader(IEnumerable<Tuple<Account, AccountRepository>> accounts, ExchangeRate exchangeRate)
		{
			var moneySum = new Money(accounts.Sum(a => a.Item1.Money.Amount), accounts.First().Item1.Money.Currency);
			var currency = accounts.First().Item1.Money.Currency;
			Title = currency != null ? currency.Code : string.Empty;
			Header.TitleText = moneySum.ToString();

			if (exchangeRate != null && exchangeRate.Rate.HasValue)
			{
				var moneyReference = new Money(moneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				Header.InfoText = moneyReference.ToString();
			}
			else {
				Header.InfoText = InternationalisationResources.NoExchangeRateFound;
			}
		}
	}
}

