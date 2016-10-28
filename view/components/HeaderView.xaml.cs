using System.Diagnostics;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public partial class HeaderView : ContentView
	{
		double initialSize;

		public string TitleText
		{
			get { return TitleLabel.Text; }
			set { TitleLabel.Text = value; setTitleSize(); }
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
				TitleLabel.Margin = new Thickness(0, -45, 0, 0);
				GridView.RowDefinitions.Clear();
				GridView.RowDefinitions.Add(new RowDefinition { Height = 100 });
			}

			initialSize = TitleLabel.FontSize;
		}

		void setTitleSize()
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
	}
}