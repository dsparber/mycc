using System.Linq;
using MyCC.Forms.Helpers;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Header
{
    public partial class HeaderView
    {
        private readonly double _defaultSize = App.ScreenHeight > 480 ? 36 : 28;
        private readonly double _defaultSizeInfoText = App.ScreenHeight > 480 ? 18 : 15;
        private readonly double _minSizeInfoText = App.ScreenHeight > 480 ? 16 : 13;
        private readonly double _minSizeMainText = App.ScreenHeight > 480 ? 28 : 24;

        private HeaderItem _data;

        public HeaderItem Data
        {
            private get => _data ?? new HeaderItem(string.Empty, string.Empty);
            set => Device.BeginInvokeOnMainThread(() =>
            {
                _data = value;
                TitleLabel.Text = GetText(Data.MainText);
                InfoText = GetText(Data.InfoText);
                AdaptSize();
            });

        }

        public string Info
        {
            set => Device.BeginInvokeOnMainThread(() =>
            {
                InfoText = GetText(value);
                AdaptSize();
            });
        }

        public string Title
        {
            set => Device.BeginInvokeOnMainThread(() =>
            {
                TitleLabel.Text = GetText(value);
                AdaptSize();
            });
        }

        private string InfoText
        {
            set
            {
                InfoLabel.Text = GetText(value);
                InfoLabelStack.Children.Clear();
                foreach (var part in GetText(value).Split('/').Select(s => s.Trim()))
                {
                    InfoLabelStack.Children.Add(new Label
                    {
                        Text = part,
                        FontSize = _defaultSizeInfoText,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.White,
                        LineBreakMode = LineBreakMode.TailTruncation
                    });
                }
            }
        }

        public string LoadingText
        {
            set => RefreshingLabel.Text = GetText(value);
        }

        public bool IsLoading
        {
            set
            {
                LoadingPanel.IsVisible = value;
                LoadingIndicator.IsRunning = value;
                InfoLabel.IsVisible = !value && Height <= 150;
                InfoLabelStack.IsVisible = !value && Height > 150;
            }
        }

        public HeaderView(bool subscribeToRefresh) : this()
        {
            if (!subscribeToRefresh) return;

            MessagingCenter.Subscribe<string>(this, Messaging.Status.Progress, d => Progress = double.Parse(d));
        }

        private double Progress
        {
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var task = ProgressBar.ProgressTo(value, 100, Easing.Linear);
                    if (value > 0.98)
                    {
                        task.ContinueWith(t => Device.BeginInvokeOnMainThread(() => ProgressBar.Progress = 0));
                    }
                });
            }
        }

        public HeaderView()
        {
            InitializeComponent();

            LoadingIndicator.HeightRequest = _defaultSizeInfoText;
            LoadingIndicator.WidthRequest = _defaultSizeInfoText;
            InfoLabel.FontSize = _defaultSizeInfoText;

            Padding = new Thickness(0, 0, 0, 10);
            WidthRequest = App.ScreenHeight / 3.0;
            HeightRequest = 90;
        }

        private static string GetText(string text) => string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            InfoLabel.IsVisible = height <= 150;
            InfoLabelStack.IsVisible = height > 150;
            AdaptSize();
        }

        private static readonly ITextSizeHelper SizeHelper = DependencyService.Get<ITextSizeHelper>();

        private void AdaptSize()
        {
            var size = (float)_defaultSize;
            var sizeInfo = (float)_defaultSizeInfoText;
            var availableWidth = Width - 48;

            while (SizeHelper.CalculateWidth(Data.MainText, size, true).Item2 > availableWidth && size > _minSizeMainText)
            {
                size -= 0.25f;
            }
            while (SizeHelper.CalculateWidth(Data.InfoText, sizeInfo).Item2 > availableWidth && sizeInfo > _minSizeInfoText)
            {
                sizeInfo -= 0.25f;
            }

            TitleLabel.FontSize = size;
            InfoLabel.FontSize = sizeInfo;
        }
    }
}