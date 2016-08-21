using MyCryptos.resources;
using Xamarin.Forms;
using System;
using System.Linq;
using data.storage;
using System.Threading.Tasks;
using models;
using data.settings;

namespace view
{
	public class TabContainerView : TabbedPage
	{ 
		public TabContainerView()
		{
			Title = InternationalisationResources.AppName;
			var tagPage = new NavigationPage(new TagsView()) { Title = InternationalisationResources.TagsTitle, Icon = "tags.png" };
			var coinPage = new NavigationPage(new CoinsView()) { Title = InternationalisationResources.CoinsTitle, Icon = "coins.png" };
			var accountPage = new NavigationPage(new AccountsView()) { Title = InternationalisationResources.AccountsTitle, Icon = "accounts.png" };
			var settingsPage = new NavigationPage(new SettingsView()) { Title = InternationalisationResources.SettingsTitle, Icon = "settings.png" };

			Children.Add(tagPage);
			Children.Add(coinPage);
			Children.Add(accountPage);
			if (Device.OS == TargetPlatform.Android)
			{
				var settingsToolbarItem = new ToolbarItem { Text = InternationalisationResources.Settings };
				settingsToolbarItem.Clicked += (sender, e) => Navigation.PushModalAsync(settingsPage);
				ToolbarItems.Add(settingsToolbarItem);
			}
			else {
				Children.Add(settingsPage);
			}

			CurrentPage = coinPage;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			var accountFastFetchTask = AccountStorage.Instance.FetchFast();
			var currencyFastFetchTask = CurrencyStorage.Instance.FetchFast();
			var exchangeRateFastFetchTask = ExchangeRateStorage.Instance.FetchFast();

			var accountFetchTask = accountFastFetchTask.ContinueWith(async t => await AccountStorage.Instance.Fetch());
			var currencyFetchTask = currencyFastFetchTask.ContinueWith(async t => await CurrencyStorage.Instance.Fetch());
			var exchangeRateFetchTask = exchangeRateFastFetchTask.ContinueWith(async t =>
			{
				await ExchangeRateStorage.Instance.Fetch();
				await await accountFetchTask;
				var neededRates = (await AccountStorage.Instance.AllElements()).Select(a => new ExchangeRate(a.Money.Currency, ApplicationSettings.BaseCurrency)).Distinct();
				await Task.WhenAll(neededRates.ToList().Select(r => ExchangeRateStorage.Instance.FetchExchangeRate(r)));
			});

			await Task.WhenAll(accountFastFetchTask, currencyFastFetchTask, exchangeRateFastFetchTask);
			await UpdateViews();

			await Task.WhenAll(accountFetchTask, currencyFetchTask, exchangeRateFetchTask);
			await UpdateViews();
		}

		async Task UpdateViews()
		{        
			var coinsView = (CoinsView)((NavigationPage)Children.ToList().Find(c => c is NavigationPage &&  ((NavigationPage)c).CurrentPage is CoinsView)).CurrentPage;
			var accountsView = (AccountsView)((NavigationPage)Children.ToList().Find(c => c is NavigationPage && ((NavigationPage)c).CurrentPage is AccountsView)).CurrentPage;

			await coinsView.UpdateView();
			await accountsView.UpdateView();
		}
	}
}


