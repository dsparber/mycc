﻿using System.Linq;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Forms.Messages;
using MyCC.Forms.view.container;
using MyCC.Forms.view.overlays;
using MyCC.Forms.view.pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyCC.Forms
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
                startPage = new PasswordView(startPage);
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
            if (page is PasswordView) return;

            page?.Navigation.PushModalAsync(new PasswordView(true), false);
        }

        protected override async void OnResume()
        {
            var passwordView = (GetCurrentPage() as PasswordView);
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
