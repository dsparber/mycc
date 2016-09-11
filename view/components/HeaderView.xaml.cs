using Xamarin.Forms;

namespace view.components
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

		public bool IsLoading
		{
			get { return LoadingPanel.IsVisible; }
			set { LoadingPanel.IsVisible = value; LoadingIndicator.IsRunning = value; InfoLabel.IsVisible = !value; }
		}

		public HeaderView()
		{
			InitializeComponent();
		}
	}
}

