using System;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class AccountDetailView
	{
		private FunctionalAccount account;
		private readonly CoinHeaderComponent header;
		private ReferenceCurrenciesView referenceView;

		public AccountDetailView(FunctionalAccount account)
		{
			InitializeComponent();

			this.account = account;

			header = new CoinHeaderComponent(account);
			ChangingStack.Children.Insert(0, header);
			referenceView = new ReferenceCurrenciesView(account.Money);
			Content.Content = referenceView;

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

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}
		}

		private void SetView()
		{
			if (Device.OS != TargetPlatform.Android)
			{
				Title = account.Money.Currency.Code;
			}

			Update();
		}

		private void Update(bool loading = false)
		{
			referenceView.UpdateView();
			Device.BeginInvokeOnMainThread(() => header.IsLoading = loading);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			referenceView.OnAppearing();
		}

		private async void Refresh(object sender, EventArgs args)
		{
			await AppTaskHelper.FetchBalanceAndRates(account as OnlineFunctionalAccount);
			await AppTaskHelper.FetchMissingRates();
		}
	}
}

