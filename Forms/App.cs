using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Helpers;
using MyCC.Core.Preperation;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using MyCC.Forms.View.Pages;
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
            if (Prepare.PreparingNeeded)
            {
                Prepare.ExecutePreperations();
            }

            var startPage = new TabContainerView() as Page;

            if (ApplicationSettings.IsPinSet)
            {
                startPage = new PasswordOverlay(true);
            }

            MainPage = ApplicationSettings.AppInitialised ? startPage : new PreparationView();

            DependencyService.Get<ILocalise>().SetLocale();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            if (!ApplicationSettings.AppInitialised) return;
			if (Migrate.MigrationsNeeded) await Migrate.ExecuteMigratations();


			// Subscribe to finished loading
			Messaging.Loading.SubscribeFinished(this, async () =>
            {
                try
                {
                    // Update only if auto refresh is enabled
                    if (ApplicationSettings.AutoRefreshOnStartup && CrossConnectivity.Current.IsConnected)
                    {
                        await AppTaskHelper.FetchBalancesAndRates();
                    }
                }
                catch (Exception e)
                {
                    e.LogError();
                }
            });

            // Load data from Database
            await Task.Run(async () =>
            {
                try
                {
                    await ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished);
                }
                catch (Exception e)
                {
                    e.LogError();
                }
            });

            // Updating available currencies and rates
            await Task.Run(async () =>
            {
                try
                {
                    if (CrossConnectivity.Current.IsConnected)
                        await ApplicationTasks.FetchCurrenciesAndAvailableRates(
                            Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted,
                            Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
                }
                catch (Exception e)
                {
                    e.LogError();
                }
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

