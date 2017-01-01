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

			if (amount != null)
			{
				Content.Children.Remove(Table);
			}

			LoadData(false);

			referenceView = new ReferenceCurrenciesView(MoneySum);
			Content.Children.Add(referenceView);

			Subscribe();
			UpdateView();

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
			else if (updateView)
			{
				UpdateView();
			}
		}

		private void UpdateView()
		{
			referenceView.UpdateView();

			if (Content.Children.Contains(Table))
			{
				var cells = accounts.Select(a => new AccountViewCell(Navigation) { Account = a.Item1, Repository = a.Item2 });

				Device.BeginInvokeOnMainThread(() =>
				{
					Table.HeightRequest = 46 * cells.Count() + 100;
					SortHelper.ApplySortOrder(cells, AccountSection, Core.Types.SortOrder.Alphabetical, Core.Types.SortDirection.Ascending);
				});
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

			referenceView.OnAppearing();
		}
	}
}

