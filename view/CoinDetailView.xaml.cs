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
			TotalMoneyLabel.Text = moneySum.ToString(); // TODO Create unified HeaderView -> Title, Info, Loading

			if (exchangeRate.Rate.HasValue)
			{
				var moneyReference = new Money(moneySum.Amount * exchangeRate.Rate.Value, exchangeRate.SecondaryCurrency);
				ReferenceMoneyLabel.Text = moneyReference.ToString();
			}
			else { 
				ReferenceMoneyLabel.Text = InternationalisationResources.NoExchangeRateFound;
			}

		}
	}
}

