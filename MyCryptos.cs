using System.Threading.Tasks;
using System.Linq;
using data.storage;
using resources;
using view;
using Xamarin.Forms;
using MyCryptos.message;
using data.settings;
using tasks;

namespace MyCryptos
{
    public class App : Application
    {
		public ErrorMessageHandler errorMessageHandler;

        public App()
        {
			errorMessageHandler = ErrorMessageHandler.Instance;

            // The root page of your application
            if (Device.OS == TargetPlatform.Android)
            {
                MainPage = new MasterDetailContainerView();
            }
            else
            {
                MainPage = new TabContainerView();
            }

            if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
            {
                DependencyService.Get<ILocalise>().SetLocale();
            }

			if (ApplicationSettings.AutoRefreshOnStartup)
			{
				AppTasks.Instance.StartFetchTask(true);
			}
			else {
				AppTasks.Instance.StartFastFetchTask();
			}
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

