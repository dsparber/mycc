using MyCryptos.Forms.helpers;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public partial class HeaderView
	{
		public string TitleText
		{
			private get { return TitleLabel.Text; }
			set
			{
				TitleLabel.Text = GetText(value);
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

			TitleText = TitleText;
			InfoText = InfoText;
			LoadingText = LoadingText;
		}

		private static string GetText(string text)
		{
			text = text?.Trim();
			return string.IsNullOrEmpty(text) ? " " : text;
		}
	}
}