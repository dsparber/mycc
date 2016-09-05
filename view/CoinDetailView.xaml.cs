using System.Collections.Generic;
using models;
using Xamarin.Forms;
using System.Linq;
using data.settings;
using MyCryptos.resources;

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

		}
	}
}

