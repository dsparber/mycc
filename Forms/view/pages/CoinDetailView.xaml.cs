using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class CoinDetailView
	{
		private ReferenceCurrenciesView referenceView;
		private AccountsTableComponent accountsView;

		private IEnumerable<Tuple<FunctionalAccount, AccountRepository>> accounts;

		private readonly Currency currency;
		private readonly decimal? amount;
		private Money MoneySum => amount != null ? new Money(amount.Value, currency) : (accounts.ToList().Count == 0) ? null : new Money(accounts.Sum(a => a.Item1.Money.Amount), accounts.First().Item1.Money.Currency);

		public CoinDetailView(Currency pageCurrency, decimal? amount = null)
		{
			InitializeComponent();

			var header = new CoinHeaderComponent(pageCurrency, true, amount);
			ChangingStack.Children.Insert(0, header);

			currency = pageCurrency;
			this.amount = amount;
			Title = currency.Code;

			LoadData(false);

			accountsView = new AccountsTableComponent(Navigation, currency);
			if (amount == null)
			{
				Content.Children.Add(accountsView);
			}
			referenceView = new ReferenceCurrenciesView(MoneySum);
			Content.Children.Add(referenceView);

			Subscribe();
		}

		private void LoadData()
		{
			LoadData(true);
		}

		private void LoadData(bool updateView)
		{
			var accs = AccountStorage.Instance.AllElementsWithRepositories;
			accounts = accs.Where(t => t.Item1.Money.Currency.Equals(currency)).ToList();

			if (accounts.ToList().Count == 0 && amount == null)
			{
				Navigation.RemovePage(this);
			}
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

		protected override void OnAppearing()
		{
			base.OnAppearing();

			accountsView.OnAppearing();
			referenceView.OnAppearing();
		}
	}
}

