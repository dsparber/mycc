using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyCC.Forms
{
    public class App : Application
    {
        public App()
        {
            var startPage = Device.OS == TargetPlatform.Android ? new MasterDetailContainer() as Page : new TabContainerView();

            if (ApplicationSettings.IsPinSet)
            {
                startPage = new PasswordView(true);
            }

            MainPage = startPage;

            DependencyService.Get<ILocalise>().SetLocale();


            if (ApplicationSettings.FirstLaunch)
            {
                Task.Run(async () =>
                {
                    await ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished);
                    await ApplicationTasks.FetchCurrenciesAndAvailableRates(
                        Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted,
                        Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
                });
            }
            else
            {
                if (ApplicationSettings.AutoRefreshOnStartup)
                {
                    Messaging.UpdatingRates.SubscribeFinished(this, async () =>
                    {
                        Messaging.UpdatingRates.Unsubscribe(this);
                        await ApplicationTasks.FetchCurrenciesAndAvailableRates(Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted, Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
                    });
                    Messaging.Loading.SubscribeFinished(this,
                        async () =>
                        {
                            ApplicationSettings.DataLoaded = true;
                            await AppTaskHelper.FetchBalancesAndRates();
                        });
                }
                else
                {
                    Messaging.Loading.SubscribeFinished(this, async () =>
                    {
                        ApplicationSettings.DataLoaded = true;
                        await ApplicationTasks.FetchCurrenciesAndAvailableRates(
                            Messaging.UpdatingCurrenciesAndAvailableRates.SendStarted,
                            Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished, ErrorOverlay.Display);
                    });
                }
                Task.Run(async () => await ApplicationTasks.LoadEverything(Messaging.Loading.SendFinished));
            }
        }

        protected override void OnSleep()
        {
            if (!ApplicationSettings.IsPinSet) return;

            var page = GetCurrentPage();
            if (page is PasswordView) return;

            page?.Navigation.PushModalAsync(new PasswordView(false, true), false);
        }

        protected override async void OnResume()
        {
            var passwordView = GetCurrentPage() as PasswordView;
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

