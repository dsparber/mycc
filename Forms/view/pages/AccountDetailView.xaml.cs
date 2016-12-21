using System;
using System.Collections.Generic;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using MyCryptos.Forms.view.components.cells;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class AccountDetailView
	{
		private readonly CurrencyEntryCell currencyEntryCell;
		private readonly List<ReferenceValueViewCell> referenceValueCells;

		private FunctionalAccount account;
		private readonly AccountRepository repository;
		private readonly CoinHeaderComponent header;


		public AccountDetailView(FunctionalAccount account, AccountRepository repository)
		{
			InitializeComponent();

			this.account = account;
			this.repository = repository;

			header = new CoinHeaderComponent(account);
			ChangingStack.Children.Insert(0, header);

			currencyEntryCell = new CurrencyEntryCell(Navigation) { IsAmountEnabled = true };

			referenceValueCells = new List<ReferenceValueViewCell>();
			SetView();

			if (!(account is OnlineFunctionalAccount))
			{
				ToolbarItems.Remove(RefreshItem);
			}

			Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => Update());
			Messaging.ReferenceCurrencies.SubscribeValueChanged(this, () => Update());

			Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => Update(true), () => Update(false));
			Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Update(true), () => Update(false));
			Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Update(true), () => Update(false));

			currencyEntryCell.OnSelected = (c) => header.TitleText = currencyEntryCell.SelectedMoney.ToString();
			currencyEntryCell.OnTyped = (m) => header.TitleText = m.ToString();

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}
		}

		private void SetView()
		{
			if (Device.OS != TargetPlatform.Android)
			{
				Title = account.Name;
			}
			currencyEntryCell.SelectedMoney = account.Money;

			Update();
		}

		private void Update(bool loading = false)
		{
			var table = new ReferenceCurrenciesSection(account.Money);
			referenceValueCells.Clear();

			Device.BeginInvokeOnMainThread(() =>
			{
				EqualsSection.Clear();
				foreach (var cell in table.Cells)
				{
					referenceValueCells.Add(cell);
					EqualsSection.Add(cell);
				}
				header.IsLoading = loading;
			});
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (account == null) return;
		}

		private async void Refresh(object sender, EventArgs args)
		{
			await AppTaskHelper.FetchBalanceAndRates(account as OnlineFunctionalAccount);
			await AppTaskHelper.FetchMissingRates();
		}
	}
}

