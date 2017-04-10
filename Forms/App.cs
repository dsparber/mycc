using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyCC.Forms
{
    public class App : Application
    {
        public static int ScreenWidth;
        public static int ScreenHeight;

        public App()
        {
            var startPage = Device.RuntimePlatform.Equals(Device.Android) ? new MasterDetailContainer() as Page : new TabContainerView();

            if (ApplicationSettings.IsPinSet)
            {
                startPage = new PasswordOverlay(true);
            }

            MainPage = startPage;

            DependencyService.Get<ILocalise>().SetLocale();

            // Subscribe to finished loading
            Messaging.Loading.SubscribeFinished(this, async () =>
            {
                ApplicationSettings.DataLoaded = true;

                // Update only if auto refresh is enabled
                if (ApplicationSettings.AutoRefreshOnStartup && CrossConnectivity.Current.IsConnected)
                {
                    await AppTaskHelper.FetchBalancesAndRates();
                }
            });

            // Load data from Database
            Task.Run(async () => await ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished));

            // Updating available currencies and rates
            Task.Run(async () =>
            {
                if (CrossConnectivity.Current.IsConnected)
                    await ApplicationTasks.FetchCurrenciesAndAvailableRates(
                        Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted,
                        Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
            });
        }

        protected override void OnSleep()
        {
            if (!ApplicationSettings.IsPinSet) return;

            var page = GetCurrentPage();
            if (page is PasswordOverlay) return;

            if (page != null) Messaging.DarkStatusBar.Send(true);
            page?.Navigation.PushModalAsync(new PasswordOverlay(false, true), false);
        }

        protected override async void OnResume()
        {
            var passwordView = GetCurrentPage() as PasswordOverlay;
            if (passwordView != null)
            {
                await passwordView.Authenticate();
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

