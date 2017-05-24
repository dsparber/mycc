using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Sources;
using MyCC.Core.Helpers;
using MyCC.Core.Settings;
using MyCC.Forms.Resources;
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
        private readonly Page _nextPage;

        public PreparationView(Page nextPage)
        {
            InitializeComponent();
            _nextPage = nextPage;

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
                // STEP 1: Fetch available currencies
                var totalCount = CurrencyStorage.Instance.CurrencySources.Count() * 2;
                var count = 0;

                Action<ICurrencySource> setProgress = source =>
                {
                    count += 1;
                    SetStatus(0.8 * count / totalCount, string.Format(I18N.LoadingCurrenciesFrom, source.Name));
                };

                await CurrencyStorage.Instance.LoadOnline(setProgress, setProgress);


                // STEP 2: Fetch needed Rates
                await TaskHelper.FetchMissingRates(false, progress => SetStatus(0.8 + progress * 0.2, I18N.LoadingRates));

                ApplicationSettings.AppInitialised = true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PushModalAsync(_nextPage);
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
                var progress = (int)Math.Round(percentage * 100, 0);
                ProgressBar.Progress = progress;
            });
        }
    }
}