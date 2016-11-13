using System.Collections.Generic;
using MyCryptos.models;
using Xamarin.Forms;
using System.Linq;
using MyCryptos.resources;
using MyCryptos.view.components;
using helpers;
using message;
using data.repositories.account;
using System;
using data.storage;
using data.settings;
using MyCryptos.helpers;
using tasks;

namespace view
{
	public partial class CoinDetailView : ContentPage
	{
		List<AccountViewCell> Cells;
		List<ReferenceValueViewCell> ReferenceValueCells;

		List<ExchangeRate> ExchangeRates;
		IEnumerable<Tuple<Account, AccountRepository>> Accounts;

		Currency currency;
		Money moneySum { get { return new Money(Accounts.Sum(a => a.Item1.Money.Amount), Accounts.First().Item1.Money.Currency); } }

		public CoinDetailView(Currency pageCurrency)
		{
			InitializeComponent();

			currency = pageCurrency;

			subscribe();
			loadData();
		}

		void loadData()
		{
			var accs = AccountStorage.Instance.AllElementsWithRepositories;
			Accounts = accs.Where(t => t.Item1.Money.Currency.Equals(currency)).ToList();

			var currencies = ApplicationSettings.ReferenceCurrencies;
			
			ExchangeRates = new List<ExchangeRate>();
			foreach (var c in currencies)
			{
				ExchangeRates.Add(ExchangeRateHelper.GetRate(currency, c));
			}

			updateView();
		}

		void updateView()
		{
			Cells = new List<AccountViewCell>();
			foreach (var a in Accounts)
			{
				Cells.Add(new AccountViewCell(Navigation) { Account = a.Item1, Repository = a.Item2 });
			}

			var table = new ReferenceCurrenciesSection(moneySum);
			ReferenceValueCells = table.Cells;
            EqualsSection.Clear();
            foreach (var c in ReferenceValueCells)
            {
                EqualsSection.Add(c);
            }

			SortHelper.ApplySortOrder(Cells, AccountSection);

			var neededRates = ReferenceValueCells.Where(c => c.IsLoading).Select(c => c.ExchangeRate);
			AppTasks.Instance.StartMissingRatesTask(neededRates);

			setHeader();
		}

		void setHeader()
		{
            Title = currency.Code;
			Header.TitleText = moneySum.ToString();

			var exchangeRate = ExchangeRates.Find(e => e.SecondaryCurrency.Equals(ApplicationSettings.BaseCurrency));

			if (exchangeRate != null && exchangeRate.Rate.HasValue)
			{
				var moneyReference = new Money(moneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				Header.InfoText = moneyReference.ToString();
			}
			else {
				Header.InfoText = InternationalisationResources.NoExchangeRateFound;

			}
		}

		void subscribe()
		{
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, (str) =>
			{
				SortHelper.ApplySortOrder(Cells, AccountSection);
				SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection);
			});

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str =>
			{
				loadData();

				if (Accounts.ToList().Count == 0)
				{
					Navigation.RemovePage(this);
				}
				else {
					updateView();
				}
			});
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => loadData());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrencies, str => loadData());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => loadData());
		}
	}
}

