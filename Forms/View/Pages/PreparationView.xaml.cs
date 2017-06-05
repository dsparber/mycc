using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Sources;
using MyCC.Core.Helpers;
using MyCC.Core.Preperation;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using MyCC.Ui.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCC.Forms.View.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreparationView
    {

        private bool _startedLoading;

        public PreparationView()
        {
            InitializeComponent();

            Action<bool> startLoading = b =>
            {
                if (!b || _startedLoading) return;

                Device.BeginInvokeOnMainThread(() =>
                {
                    NoConnectionView.IsVisible = false;
                    ProgressView.IsVisible = true;
                });

                _startedLoading = true;
                Task.Run(LoadInitalData).ConfigureAwait(false);
            };

            if (!CrossConnectivity.Current.IsConnected)
            {
                NoConnectionView.IsVisible = true;
                ProgressView.IsVisible = false;
            }

            startLoading(CrossConnectivity.Current.IsConnected);
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) => startLoading(args.IsConnected);
        }

        private async Task LoadInitalData()
        {
            try
            {
                if (Prepare.PreparingNeeded)
                {
                    if (Prepare.AsyncExecutePreperations != null) await Prepare.AsyncExecutePreperations;
                }
                if (Migrate.MigrationsNeeded) await Migrate.ExecuteMigratations();

                // STEP 1: Fetch available currencies
                var totalCount = CurrencyStorage.Instance.Sources.Count() * 2;
                var count = 0;

                Action<ICurrencySource> setProgress = source =>
                {
                    count += 1;
                    SetStatus(0.8 * count / totalCount, string.Format(I18N.LoadingCurrenciesFrom, source.Name));
                };

                await CurrencyStorage.Instance.LoadOnline(setProgress, setProgress);

                await ApplicationTasks.LoadEverything();

                // STEP 2: Fetch needed Rates
                await TaskHelper.FetchNeededButNotLoadedRates(false, progress => SetStatus(0.8 + progress * 0.1, I18N.LoadingRates));

                ApplicationSettings.AppInitialised = true;

                // STEP 3: Refresh data if needed
                if (ApplicationSettings.AutoRefreshOnStartup)
                {
                    SetStatus(0.95, I18N.UpdatingBalancesAndRates);
                    await AppTaskHelper.FetchBalancesAndRates();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PushModalAsync(ApplicationSettings.IsPinSet ? new PasswordOverlay(true) as Page : new TabContainerView());
                });
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        private void SetStatus(double percentage, string text)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressLabel.Text = text;
                ProgressBar.Progress = percentage;
            });
        }
    }
}