using resources;
using view;
using Xamarin.Forms;
using MyCryptos.message;
using data.settings;
using tasks;
using System.Linq;

namespace MyCryptos
{
	public class App : Application
	{
		public ErrorMessageHandler errorMessageHandler;

		public App()
		{
			errorMessageHandler = ErrorMessageHandler.Instance;

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

			if (ApplicationSettings.AutoRefreshOnStartup)
			{
				AppTasks.Instance.StartFetchTask(true);
			}
			else
			{
				AppTasks.Instance.StartFastFetchTask();
			}
		}

		protected override void OnSleep()
		{
			if (ApplicationSettings.IsPinSet)
			{
				var page = GetCurrentPage();
				if (page is PasswordView) return;

				page?.Navigation.PushModalAsync(new PasswordView(true), false);
			}
		}

		protected async override void OnResume()
		{
			var view = (GetCurrentPage() as PasswordView);
			if (view != null)
			{
				await view.Authenticate();
			}
		}

		Page GetCurrentPage()
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

