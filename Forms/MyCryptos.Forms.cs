﻿using view;
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

            var loadTask = ApplicationTasks.LoadEverything();
            loadTask.ContinueWith(t => Messaging.Loading.SendFinished());

            var currencyTask = ApplicationTasks.FetchCurrenciesAndAvailableRates();
            currencyTask.ContinueWith(t => Messaging.UpdatingCurrenciesAndAvailableRates.SendFinished());

            if (!ApplicationSettings.AutoRefreshOnStartup) return;

            Messaging.UpdatingExchangeRates.SendStarted();
            var task = ApplicationTasks.FetchAllExchangeRates();
            task.ContinueWith(t => Messaging.UpdatingExchangeRates.SendFinished());

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
