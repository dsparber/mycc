using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.view.components;

namespace MyCC.Forms.view.pages
{
	public partial class CoinDetailView
	{
		private ReferenceCurrenciesView referenceView;
		private AccountsTableComponent accountsView;

		private IEnumerable<Tuple<FunctionalAccount, AccountRepository>> accounts;

		private readonly Currency currency;
		private Money MoneySum => (accounts.ToList().Count == 0) ? null : new Money(accounts.Sum(a => a.Item1.Money.Amount), accounts.First().Item1.Money.Currency);

		public CoinDetailView(Currency pageCurrency)
		{
			InitializeComponent();

			var header = new CoinHeaderComponent(pageCurrency, true);
			ChangingStack.Children.Insert(0, header);

			currency = pageCurrency;
			Title = currency.Code;

			LoadData(false);

			accountsView = new AccountsTableComponent(Navigation, currency);

			ContentView.Children.Add(accountsView);

			referenceView = new ReferenceCurrenciesView(MoneySum);
			ContentView.Children.Add(referenceView);

			Subscribe();
		}

		private void LoadData()
		{
			LoadData(true);
		}

		private void LoadData(bool updateView)
		{
			var accs = AccountStorage.Instance.AllElementsWithRepositories;
			accounts = accs.Where(t => t.Item1.Money.Currency.Code.Equals(currency.Code)).ToList();

			if (accounts.ToList().Count == 0)
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

		private void ShowInfo(object sender, EventArgs args)
		{
			Navigation.PushAsync(new CoinInfoView(currency));
		}
	}
}

