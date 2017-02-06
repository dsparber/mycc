﻿using System.Threading.Tasks;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public partial class HeaderView
    {

        private const double DefaultSize = 36;
        private const double DefaultSizeSmall = 24;

        public string TitleText
        {
            private get { return TitleLabel.Text ?? string.Empty; }
            set
            {
                TitleLabel.Text = GetText(value);
                AdaptSize();
            }
        }

        protected string TitleTextSmall
        {
            private get { return TitleLabelSmall.Text ?? string.Empty; }
            set
            {
                TitleLabelSmall.Text = GetText(value);
                AdaptSize();
            }
        }

        public string InfoText
        {
            private get { return InfoLabel.Text ?? string.Empty; }
            set
            {
                InfoLabel.Text = GetText(value);
            }
        }

        public string LoadingText
        {
            private get { return RefreshingLabel.Text; }
            set
            {
                RefreshingLabel.Text = GetText(value);
            }
        }

        public bool IsLoading
        {
            set { LoadingPanel.IsVisible = value; LoadingIndicator.IsRunning = value; InfoLabel.IsVisible = !value; }
        }

        protected HeaderView(bool subscribeToRefresh) : this()
        {
            if (!subscribeToRefresh) return;

            MessagingCenter.Subscribe<string>(this, Messaging.SetProgress, d => Progress = double.Parse(d));
        }

        private double Progress
        {
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProgressBar.ProgressTo(value, 100, Easing.Linear);
                });
                if (value > 0.98)
                {
                    Task.Delay(200).ContinueWith(t => Device.BeginInvokeOnMainThread(() => ProgressBar.Progress = 0));
                }
            }
        }

        public HeaderView()
        {
            InitializeComponent();

            LoadingIndicator.HeightRequest = 18;
            LoadingIndicator.WidthRequest = 18;
            if (Device.OS == TargetPlatform.Android)
            {
                LoadingIndicator.VerticalOptions = LayoutOptions.Center;
                ProgressBar.HeightRequest = 1;
            }

            Padding = new Thickness(0, 0, 0, 10);

            TitleText = TitleText;
            InfoText = InfoText;
            LoadingText = LoadingText;

            Messaging.Layout.SubscribeValueChanged(this, AdaptSize);
        }

        private static string GetText(string text)
        {
            text = text?.Trim();
            return string.IsNullOrEmpty(text) ? " " : text;
        }

        private void AdaptSize()
        {
            var size = (float?)DefaultSize + 0.25f;
            var sizeSmall = (float?)DefaultSizeSmall + 0.25f;
            double width, availableWidth;

            do
            {
                size -= 0.25f; sizeSmall -= 0.25f;
                width = DependencyService.Get<ITextSizeHelper>().CalculateWidth(TitleText, size, true).Item2
                        + DependencyService.Get<ITextSizeHelper>().CalculateWidth(TitleTextSmall, sizeSmall, true).Item2;
                availableWidth = Width - 40;

            } while (availableWidth > 0 && width > availableWidth);

            TitleLabel.FontSize = (double)size;
            TitleLabelSmall.FontSize = (double)sizeSmall;
        }
    }
}