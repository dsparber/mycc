using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using MyCryptos.Forms.view.components.cells;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class CoinDetailView
	{
		private List<AccountViewCell> cells;
		private List<ReferenceValueViewCell> referenceValueCells;

		private List<ExchangeRate> exchangeRates;
		private IEnumerable<Tuple<FunctionalAccount, AccountRepository>> accounts;

		private readonly Currency currency;
		private Money MoneySum => (accounts.ToList().Count == 0) ? null : new Money(accounts.Sum(a => a.Item1.Money.Amount), accounts.First().Item1.Money.Currency);

		public CoinDetailView(Currency pageCurrency)
		{
			InitializeComponent();

			var header = new CoinsHeaderView(pageCurrency, true);
			ChangingStack.Children.Insert(0, header);

			currency = pageCurrency;
			Title = currency.Code;

			Subscribe();
			LoadData();
		}

		private void LoadData()
		{
			var accs = AccountStorage.Instance.AllElementsWithRepositories;
			accounts = accs.Where(t => t.Item1.Money.Currency.Equals(currency)).ToList();

			var currencies = ApplicationSettings.ReferenceCurrencies;

			exchangeRates = new List<ExchangeRate>();
			foreach (var c in currencies)
			{
				exchangeRates.Add(ExchangeRateHelper.GetRate(currency, c));
			}
			exchangeRates.RemoveAll(e => e == null);


			if (accounts.ToList().Count == 0)
			{
				Navigation.RemovePage(this);
			}
			else
			{
				UpdateView();
			}
		}

		private void UpdateView()
		{
			cells = new List<AccountViewCell>();
			foreach (var a in accounts)
			{
				cells.Add(new AccountViewCell(Navigation) { Account = a.Item1, Repository = a.Item2 });
			}

			var table = new ReferenceCurrenciesSection(MoneySum);
			referenceValueCells = table.Cells;

			Device.BeginInvokeOnMainThread(() =>
			{
				EqualsSection.Clear();
				foreach (var c in referenceValueCells)
				{
					EqualsSection.Add(c);
				}

				SortHelper.ApplySortOrder(cells, AccountSection, Core.Types.SortOrder.Alphabetical, Core.Types.SortDirection.Ascending);
			});
		}

		private void Subscribe()
		{
			Messaging.UpdatingAccounts.SubscribeFinished(this, LoadData);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, LoadData);
			Messaging.FetchMissingRates.SubscribeFinished(this, LoadData);

			Messaging.ReferenceCurrency.SubscribeValueChanged(this, LoadData);
			Messaging.ReferenceCurrencies.SubscribeValueChanged(this, LoadData);
		}

		private async void Refresh(object sender, EventArgs args)
		{
			await AppTaskHelper.FetchBalanceAndRates(currency);
		}
	}
}

