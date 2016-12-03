using view;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Resources;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.Messages;

namespace MyCryptos
{
	public class App : Application
	{
		public App()
		{
			Page startPage;

			// The root page of your application
			if (Device.OS == TargetPlatform.Android)
			{
				startPage = new MasterDetailContainerView();
			}
			else
			{
				startPage = new TabContainerView();
			}

			if (ApplicationSettings.IsPinSet)
			{
				startPage = new PasswordView(startPage);
			}

			MainPage = startPage;

			if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
			{
				DependencyService.Get<ILocalise>().SetLocale();
			}
		}

		protected override async void OnStart()
		{
			base.OnStart();

			await ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished);

			await ApplicationTasks.FetchCurrenciesAndAvailableRates(Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);

			if (!ApplicationSettings.AutoRefreshOnStartup) return;

			Messaging.UpdatingExchangeRates.SendStarted();
			await ApplicationTasks.FetchAllExchangeRates(Messaging.UpdatingExchangeRates.SendFinished, ErrorOverlay.Display);
		}

		protected override void OnSleep()
		{
			if (!ApplicationSettings.IsPinSet) return;

			var page = GetCurrentPage();
			if (page is PasswordView) return;

			page?.Navigation.PushModalAsync(new PasswordView(true), false);
		}

		protected override async void OnResume()
		{
			var view = (GetCurrentPage() as PasswordView);
			if (view != null)
			{
				await view.Authenticate();
			}
		}

		private Page GetCurrentPage()
		{
			var page = MainPage.Navigation.ModalStack.LastOrDefault() ?? MainPage;
			if (page.Navigation.NavigationStack.Count > 0)
			{
				page = page.Navigation.NavigationStack.LastOrDefault() ?? page;
			}
			return page;
		}
	}
}

