using resources;
using view;
using Xamarin.Forms;

namespace MyCryptos
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application
			MainPage = new TabContainerView();

			if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
			{
				DependencyService.Get<ILocalise>().SetLocale();
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

