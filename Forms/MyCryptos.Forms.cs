using Xamarin.Forms;
using System.Linq;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.view.container;
using MyCryptos.Forms.view.overlays;

namespace MyCryptos
{
    public class App : Application
    {
        public App()
        {
            Page startPage;

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
                startPage = new Forms.view.pages.PasswordView(startPage);
            }

            MainPage = startPage;

            if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
            {
                DependencyService.Get<ILocalise>().SetLocale();
            }

            if (ApplicationSettings.FirstLaunch)
            {
                ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished);
                ApplicationTasks.FetchCurrenciesAndAvailableRates(Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted, Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
            }
            else
            {
                if (ApplicationSettings.AutoRefreshOnStartup)
                {
                    Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, () =>
                    {
                        Messaging.UpdatingAccountsAndRates.Unsubscribe(this);
                        ApplicationTasks.FetchCurrenciesAndAvailableRates(Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted, Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
                    });
                    Messaging.Loading.SubscribeFinished(this, () => ApplicationTasks.FetchBalancesAndRates(Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display));
                }
                else
                {
                    Messaging.Loading.SubscribeFinished(this, () => ApplicationTasks.FetchCurrenciesAndAvailableRates(Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted, Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display));
                }
                ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished);
            }
        }

        protected override void OnSleep()
        {
            if (!ApplicationSettings.IsPinSet) return;

            var page = GetCurrentPage();
            if (page is Forms.view.pages.PasswordView) return;

            page?.Navigation.PushModalAsync(new Forms.view.pages.PasswordView(true), false);
        }

        protected override async void OnResume()
        {
            var passwordView = (GetCurrentPage() as Forms.view.pages.PasswordView);
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

