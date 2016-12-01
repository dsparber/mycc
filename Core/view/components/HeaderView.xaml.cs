using MyCryptos.helpers;
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
				var size = DependencyService.Get<TextSizeHelper>().calculateWidth(GetText(value), (float)TitleLabel.FontSize, true);
				TitleLabel.HeightRequest = size.Item1;
				TitleLabel.WidthRequest = size.Item2;
			}
		}

		public string InfoText
		{
			private get { return InfoLabel.Text; }
			set
			{
				InfoLabel.Text = GetText(value);
				var size = DependencyService.Get<TextSizeHelper>().calculateWidth(GetText(value), (float)InfoLabel.FontSize);
				InfoLabel.HeightRequest = size.Item1;
				InfoLabel.WidthRequest = size.Item2;
			}
		}

		public string LoadingText
		{
			private get { return RefreshingLabel.Text; }
			set
			{
				RefreshingLabel.Text = GetText(value);
				var size = DependencyService.Get<TextSizeHelper>().calculateWidth(GetText(value), (float)RefreshingLabel.FontSize);
				RefreshingLabel.HeightRequest = size.Item1;
				RefreshingLabel.WidthRequest = size.Item2;
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