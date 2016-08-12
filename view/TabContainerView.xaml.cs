using Xamarin.Forms;

namespace view
{
	public partial class TabContainerView : TabbedPage
	{
		public TabContainerView()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			CurrentPage = Children[1];
		}
	}
}

