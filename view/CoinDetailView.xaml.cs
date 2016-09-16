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
		List<AccountViewCell> Cells;
		List<ReferenceValueViewCell> ReferenceValueCells;

		public CoinDetailView(IEnumerable<Tuple<Account, AccountRepository>> accounts, ExchangeRate exchangeRate)
		{
			InitializeComponent();

			updateView(accounts, exchangeRate);

			MessagingCenter.Subscribe<string>(this, MessageConstants.SortOrderChanged, (str) =>
			{
				SortHelper.ApplySortOrder(Cells, AccountSection);
				SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection);
			});
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdateAccounts, async (str) =>
			{
				var accs = await AccountStorage.Instance.AllElementsWithRepositories();
				accs = accs.Where(t => t.Item1.Money.Currency.Equals(currency(accounts))).ToList();

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
			Cells = new List<AccountViewCell>();
			foreach (var a in accounts)
			{
				Cells.Add(new AccountViewCell(Navigation) { Account = a.Item1, Repository = a.Item2 });
			}

			var table = new ReferenceCurrenciesTableView { BaseMoney = moneySum(accounts) };
			ReferenceValueCells = table.Cells;

			SortHelper.ApplySortOrder(Cells, AccountSection);
			SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection);

			setHeader(accounts, exchangeRate);
		}

		void setHeader(IEnumerable<Tuple<Account, AccountRepository>> accounts, ExchangeRate exchangeRate)
		{
			Title = currency(accounts) != null ? currency(accounts).Code : string.Empty;
			Header.TitleText = moneySum(accounts).ToString();

			if (exchangeRate != null && exchangeRate.Rate.HasValue)
			{
				var moneyReference = new Money(moneySum(accounts).Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				Header.InfoText = moneyReference.ToString();
			}
			else {
				Header.InfoText = InternationalisationResources.NoExchangeRateFound;
			}
		}

		Money moneySum(IEnumerable<Tuple<Account, AccountRepository>> accounts)
		{
			return new Money(accounts.Sum(a => a.Item1.Money.Amount), accounts.First().Item1.Money.Currency);
		}

		Currency currency(IEnumerable<Tuple<Account, AccountRepository>> accounts)
		{
			return accounts.First().Item1.Money.Currency;
		}
	}
}

