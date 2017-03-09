﻿using System.Linq;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
	public partial class HeaderView
	{

		private readonly double DefaultSize = App.ScreenHeight > 480 ? 36 : 28;
		private readonly double DefaultSizeInfoText = App.ScreenHeight > 480 ? 18 : 15;
		private readonly double DefaultSizeSmall = App.ScreenHeight > 480 ? 24 : 20;

		public string TitleText
		{
			private get { return TitleLabel.Text ?? string.Empty; }
			set
			{
				TitleLabel.Text = GetText(value);
				AdaptSize();
			}
		}

		public string HeadingText
		{
			set { HeadingLabel.Text = GetText(value); }
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
				InfoLabelStack.Children.Clear();
				foreach (var part in GetText(value).Split('/').Select(s => s.Trim()))
				{
					InfoLabelStack.Children.Add(new Label
					{
						Text = part,
						FontSize = DefaultSizeInfoText,
						HorizontalTextAlignment = TextAlignment.Center,
						TextColor = Color.White,
						LineBreakMode = LineBreakMode.TailTruncation
					});
				}
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

			MessagingCenter.Subscribe<string>(this, Messaging.Progress, d => Progress = double.Parse(d));
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

			LoadingIndicator.HeightRequest = DefaultSizeInfoText;
			LoadingIndicator.WidthRequest = DefaultSizeInfoText;
			InfoLabel.FontSize = DefaultSizeInfoText;

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

			WidthRequest = App.ScreenHeight / 3;
		}

		private static string GetText(string text)
		{
			text = text?.Trim();
			return string.IsNullOrEmpty(text) ? " " : text;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			InfoLabel.IsVisible = height <= 150;
			InfoLabelStack.IsVisible = height > 150;
			HeadingLabel.IsVisible = height > 150;
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

			HeightRequest = 90;
		}
	}
}