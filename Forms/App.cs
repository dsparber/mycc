﻿using System.Linq;
using MyCC.Core.Settings;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using MyCC.Forms.View.Pages;
using MyCC.Ui;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyCC.Forms
{
    public class App : Application
    {
        public static int ScreenHeight;

        public App()
        {
            DependencyService.Get<ILocalise>().SetLocale();


            if (UiUtils.Prepare.PreparingNeeded)
            {
                MainPage = new PreparationView();
            }
            else
            {
                UiUtils.Update.LoadNeededDataFromDatabase();

                MainPage = ApplicationSettings.IsPinSet ? new PasswordOverlay(true) : new TabContainerView() as Page;

                if (CrossConnectivity.Current.IsConnected) UiUtils.Update.FetchCurrencies();
            }
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

