using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public partial class HeaderView
    {
        private readonly double initialSize;

        public string TitleText
        {
            private get { return TitleLabel.Text; }
            set { TitleLabel.Text = GetText(value); SetTitleSize(); }
        }

        public string InfoText
        {
            private get { return InfoLabel.Text; }
            set { InfoLabel.Text = GetText(value); }
        }

        public string LoadingText
        {
            private get { return RefreshingLabel.Text; }
            set { RefreshingLabel.Text = GetText(value); }
        }

        public bool IsLoading
        {
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

            initialSize = TitleLabel.FontSize;

            TitleText = TitleText;
            InfoText = InfoText;
            LoadingText = LoadingText;
        }

        private void SetTitleSize()
        {
            double factor = 1;
            if (TitleText.Length > 23)
            {
                factor = 0.6;
            }
            else if (TitleText.Length > 19)
            {
                factor = 0.7;
            }
            else if (TitleText.Length > 15)
            {
                factor = 0.8;
            }
            TitleLabel.FontSize = initialSize * factor;
        }

        private static string GetText(string text)
        {
            text = text?.Trim();
            return string.IsNullOrEmpty(text) ? " " : text;
        }
    }
}