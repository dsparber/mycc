using System;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public partial class HeaderView
	{
		private double defaultSize = 36;

		public string TitleText
		{
			private get { return TitleLabel.Text; }
			set
			{
				TitleLabel.Text = GetText(value);
				AdaptSize(GetText(value));
			}
		}

		public string InfoText
		{
			private get { return InfoLabel.Text; }
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
			get { return LoadingIndicator.IsRunning; }
			set { LoadingPanel.IsVisible = value; LoadingIndicator.IsRunning = value; InfoLabel.IsVisible = !value; }
		}

		public HeaderView()
		{
			InitializeComponent();

			LoadingIndicator.HeightRequest = 18;
			LoadingIndicator.WidthRequest = 18;
			if (Device.OS == TargetPlatform.Android)
			{

				LoadingIndicator.VerticalOptions = LayoutOptions.Center;
			}

			Padding = new Thickness(0, 0, 0, 10);

			TitleText = TitleText;
			InfoText = InfoText;
			LoadingText = LoadingText;

			Messaging.Layout.SubscribeValueChanged(this, () => AdaptSize(TitleText));
		}

		private static string GetText(string text)
		{
			text = text?.Trim();
			return string.IsNullOrEmpty(text) ? " " : text;
		}

		void AdaptSize(string text)
		{
			var size = (float?)defaultSize + 0.25f;
			double width = 0, availableWidth = 0;

			do
			{
				size -= 0.25f;
				width = DependencyService.Get<ITextSizeHelper>().CalculateWidth(text, size, true).Item2;
				availableWidth = Width - 40;

			} while (availableWidth > 0 && width > availableWidth);

			TitleLabel.FontSize = (double)size;
		}
	}
}