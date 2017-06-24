using System.Threading.Tasks;
using MyCC.Core.Settings;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using MyCC.Ui;
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

            void StartLoading(bool b)
            {
                if (!b || _startedLoading) return;

                Device.BeginInvokeOnMainThread(() =>
                {
                    NoConnectionView.IsVisible = false;
                    ProgressView.IsVisible = true;
                });

                _startedLoading = true;
                Task.Run(LoadInitalData).ConfigureAwait(false);
            }

            if (!CrossConnectivity.Current.IsConnected)
            {
                NoConnectionView.IsVisible = true;
                ProgressView.IsVisible = false;
            }

            StartLoading(CrossConnectivity.Current.IsConnected);
            CrossConnectivity.Current.ConnectivityChanged += (sender, args) => StartLoading(args.IsConnected);
        }

        private async Task LoadInitalData()
        {
            await UiUtils.Prepare.Prepare(tuple => SetStatus(tuple.progress, tuple.infoText));

            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushModalAsync(ApplicationSettings.IsPinSet ? new PasswordOverlay(true) as Page : new TabContainerView());
            });
        }

        private void SetStatus(double percentage, string text)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressLabel.Text = text;
                ProgressBar.ProgressTo(percentage, 500, Easing.CubicOut);
            });
        }
    }
}