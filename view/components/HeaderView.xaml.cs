using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public partial class HeaderView : ContentView
    {
        public string TitleText
        {
            get { return TitleLabel.Text; }
            set { TitleLabel.Text = value; }
        }

        public string InfoText
        {
            get { return InfoLabel.Text; }
            set { InfoLabel.Text = value; }
        }

        public string LoadingText
        {
            get { return RefreshingLabel.Text; }
            set { RefreshingLabel.Text = value; }
        }

        public bool IsLoading
        {
            get { return LoadingPanel.IsVisible; }
            set { LoadingPanel.IsVisible = value; LoadingIndicator.IsRunning = value; InfoLabel.IsVisible = !value; }
        }

        public HeaderView()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.Android)
            {
                LoadingIndicator.HeightRequest = 18;
                LoadingIndicator.WidthRequest = 18;
                LoadingIndicator.VerticalOptions = LayoutOptions.Center;
            }
        }
    }
}